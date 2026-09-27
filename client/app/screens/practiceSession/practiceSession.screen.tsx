import Ionicons from '@expo/vector-icons/Ionicons';
import { RouteProp, useNavigation, useRoute } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import * as Haptics from 'expo-haptics';
import React, { JSX, useCallback, useEffect, useLayoutEffect, useRef, useState } from 'react';
import { ActivityIndicator, Dimensions, Modal, Pressable, ScrollView, Text, TouchableOpacity, View } from 'react-native';
import { RootStackParamList } from '../../../types/router';
import { completePractice } from '../../database/repositories/practice.repository';
import { preferencesQuery } from '../../database/repositories/userPreferences.repository';
import useGlobalStyles from '../../styles/gobalStyles';
import useAppTheme from '../../theme';
import { getBookName, getVerseNumbers } from '../../utils/referenceUtils';
import { showToast } from '../../utils/toast';

const TOTAL_STAGES = 4;

const NUMBER_KEYS = '1234567890'.split('');
const LETTER_ROWS = ['qwertyuiop', 'asdfghjkl', 'zxcvbnm'].map((row) => row.split(''));

const KeyboardButton = React.memo(function KeyboardButton({
  char,
  height,
  onPress,
  backgroundColor,
  textColor,
}: {
  char: string;
  height: number;
  onPress: (char: string) => void;
  backgroundColor: string;
  textColor: string;
}) {
  return (
    <Pressable
      onPress={() => onPress(char)}
      style={{ width: '8%', height, backgroundColor, borderRadius: 8, justifyContent: 'center', alignItems: 'center', margin: 3 }}
    >
      <Text style={{ color: textColor, fontSize: 24 }}>{char}</Text>
    </Pressable>
  );
});

interface Word {
  id: number;
  word: string;
  isCorrect: boolean | null;
  isHint: boolean;
}

export default function PracticeSessionScreen() {
  const styles = useGlobalStyles();
  const theme = useAppTheme();
  const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
  const route = useRoute<RouteProp<RootStackParamList, 'practiceSession'>>();
  const userPassage = route.params.userPassage;
  const [dateOpened] = useState(() => new Date().toISOString());

  const [typeOutReference, setTypeOutReference] = useState(false);
  const [allWords, setAllWords] = useState<Word[]>([]);
  const [referenceWords, setReferenceWords] = useState<Word[]>([]);
  const [currentStage, setCurrentStage] = useState(1);
  const [loading, setLoading] = useState(true);

  const [readableReference, setReadableReference] = useState('');
  const [book, setBook] = useState('');
  const [chapter, setChapter] = useState('');
  const [readableReferenceVerses, setReadableReferenceVerses] = useState('');
  const [firstStageWords, setFirstStageWords] = useState<Word[]>([]);

  const [stageHiddenIndeces, setStageHiddenIndices] = useState<number[][]>([]);
  const [activeIndex, setActiveIndex] = useState(0);
  const [activeReferenceIndex, setActiveReferenceIndex] = useState(0);
  const [bookWords, setBookWords] = useState<Word[]>([]);
  const [activeBookIndex, setActiveBookIndex] = useState(0);
  const [learnedBook, setLearnedBook] = useState(false);
  const [learnedVerseReference, setLearnedVerseReference] = useState(false);

  const [accuracyModalVisible, setAccuracyModalVisible] = useState(false);
  const [accuracy, setAccuracy] = useState(0);
  const [summaryModalVisible, setSummaryModalVisible] = useState(false);
  const [stageAccuracies, setStageAccuracies] = useState<number[]>([]);

  useEffect(() => {
    const getPassagePracticing = async () => {
      const [preferences] = await preferencesQuery();

      const shouldTypeReference = preferences?.typeOutReference ?? false;
      const { reference } = userPassage.passage;
      const verses = [...userPassage.passage.verses]
        .sort((a, b) => (getVerseNumbers(a.reference)[0] ?? 0) - (getVerseNumbers(b.reference)[0] ?? 0));

      const bookValue = getBookName(reference);
      const chapterValue = reference.chapter.toString();
      const versesValue = reference.readableReference.substring(reference.readableReference.indexOf(':') + 1);
      const passageTextValue = verses.map((v) => v.translationContents?.[0]?.plainText ?? v.text).join(' ');

      setTypeOutReference(shouldTypeReference);
      setReadableReference(reference.readableReference);
      setBook(bookValue);
      setChapter(chapterValue);
      setReadableReferenceVerses(versesValue);

      const words: Word[] = [];
      const referenceWordsArray: Word[] = [];
      const bookWordsArray: Word[] = [];
      let counter = 0;

      if (shouldTypeReference && bookValue) {
        bookValue.trim().split(/\s+/).forEach((word) => {
          bookWordsArray.push({ id: counter++, word: word.charAt(0), isHint: true, isCorrect: null });
        });
        setBookWords(bookWordsArray);
        setActiveBookIndex(0);
        setLearnedBook(false);
      }

      passageTextValue.split(/\s+/).filter(Boolean).forEach((word) => {
        words.push({ id: counter++, isHint: true, word: word + ' ', isCorrect: null });
      });

      if (shouldTypeReference) {
        `${chapterValue}${versesValue}`.replace(/\D/g, '').split('').forEach((digit) => {
          referenceWordsArray.push({ id: counter++, word: digit, isHint: true, isCorrect: null });
        });
        setReferenceWords(referenceWordsArray);
      }

      setFirstStageWords(words);

      const totalLength = words.length;
      const stageHiddenIndices: number[][] = [];

      const percentStage1 = 0.00;
      const percentIncrement = 0.30;

      const getRandomUnique = (count: number, max: number): number[] => {
        const set = new Set<number>();
        while (set.size < count) {
          set.add(Math.floor(Math.random() * max));
        }
        return Array.from(set);
      };

      const stage1Count = Math.floor(totalLength * percentStage1);
      const stage1 = getRandomUnique(stage1Count, totalLength);
      stageHiddenIndices.push(stage1);

      const stage2Count = Math.floor(totalLength * (percentStage1 + percentIncrement));
      const stage2New = getRandomUnique(stage2Count - stage1.length, totalLength);
      const stage2 = Array.from(new Set([...stage1, ...stage2New]));
      stageHiddenIndices.push(stage2);

      const stage3Count = Math.floor(totalLength * (percentStage1 + percentIncrement * 2.5));
      const stage3New = getRandomUnique(stage3Count - stage2.length, totalLength);
      const stage3 = Array.from(new Set([...stage2, ...stage3New]));
      stageHiddenIndices.push(stage3);

      const stage4 = Array.from({ length: totalLength }, (_, i) => i);
      stageHiddenIndices.push(stage4);

      setStageHiddenIndices(stageHiddenIndices);
      setActiveIndex(0);
      setLoading(false);
    };

    getPassagePracticing();
  }, [userPassage]);

  const renderReference = () => {
    const display: JSX.Element[] = [];
    let refIndex = 0;
    let bookIndex = 0;

    const restPart = ` ${chapter}:${readableReferenceVerses}`;

    const bookWordsSplit = book.trim().split(/\s+/);
    for (let i = 0; i < bookWordsSplit.length; i++) {
      const word = bookWordsSplit[i];
      if (word.length > 0) {
        const bookWord = bookWords[bookIndex];
        const isCorrect = bookWord?.isCorrect ?? null;
        const isHint = bookWord?.isHint ?? true;

        let color = theme.colors.verseHint;

        if (isCorrect === true) {
          color = theme.colors.onBackground;
        } else if (isCorrect === false) {
          color = theme.colors.notification;
        } else if (isHint) {
          color = theme.colors.verseHint;
        } else {
          color = 'transparent';
        }

        display.push(
          <Text key={`book-char-${i}`} style={{ color }}>
            {word.charAt(0)}
          </Text>
        );

        if (word.length > 1 && isCorrect !== null) {
          display.push(
            <Text key={`book-rest-${i}`} style={{ color: theme.colors.onBackground }}>
              {word.slice(1)}
            </Text>
          );
        }

        bookIndex++;

        if (i < bookWordsSplit.length - 1 && isCorrect !== null) {
          display.push(
            <Text key={`book-space-${i}`} style={{ color: theme.colors.onBackground }}>
              {' '}
            </Text>
          );
        }
      }
    }

    if (learnedBook) {
      for (let i = 0; i < restPart.length; i++) {
        const ch = restPart[i];

        if (/\d/.test(ch)) {
          const refWord = referenceWords[refIndex];
          const isCorrect = refWord?.isCorrect ?? null;
          const isHint = refWord?.isHint ?? false;

          let color = 'transparent';

          if (isCorrect === true) {
            color = theme.colors.onBackground;
          } else if (isCorrect === false) {
            color = theme.colors.notification;
          } else if (isHint) {
            color = theme.colors.verseHint;
          }

          display.push(
            <Text key={`digit-${i}`} style={{ color }}>
              {ch}
            </Text>
          );

          refIndex++;
        } else {
          display.push(
            <Text key={`punct-${i}`} style={{ color: theme.colors.onBackground }}>
              {ch}
            </Text>
          );
        }
      }
    }

    return display;
  };

  const resetReferenceForStage = (stage: number) => {
    if (!typeOutReference) return;

    const hideReference = stage >= 3;

    setReferenceWords(prev =>
      prev.map(rw => ({
        ...rw,
        isCorrect: null,
        isHint: !hideReference,
      }))
    );

    setBookWords(prev =>
      prev.map(bw => ({
        ...bw,
        isCorrect: null,
        isHint: !hideReference,
      }))
    );

    setActiveReferenceIndex(0);
    setActiveBookIndex(0);
    setLearnedVerseReference(false);
    setLearnedBook(false);
  };

  const handleKeyboardPress = (char: string) => {
    Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Rigid);

    if (typeOutReference && activeBookIndex < bookWords.length && !learnedBook) {
      const currentBookWord = bookWords[activeBookIndex];
      const expectedChar = currentBookWord.word.trim().charAt(0).toLowerCase();

      if (char.toLowerCase() !== expectedChar) {
        Haptics.notificationAsync(Haptics.NotificationFeedbackType.Error);
      }

      const updatedBookWords = [...bookWords];
      updatedBookWords[activeBookIndex] = {
        ...currentBookWord,
        isCorrect: char.toLowerCase() === expectedChar,
      };
      setBookWords(updatedBookWords);

      setActiveBookIndex(activeBookIndex + 1);

      if (activeBookIndex + 1 >= bookWords.length) {
        setActiveBookIndex(0);
        setLearnedBook(true);
        setActiveReferenceIndex(0);
      }

      return;
    }

    if (typeOutReference && activeReferenceIndex < referenceWords.length && !learnedVerseReference && learnedBook) {
      const currentRefWord = referenceWords[activeReferenceIndex];
      const expectedChar = currentRefWord.word.trim().charAt(0).toLowerCase();

      if (char.toLowerCase() !== expectedChar) {
        Haptics.notificationAsync(Haptics.NotificationFeedbackType.Error);
      }

      const updatedReferenceWords = [...referenceWords];
      updatedReferenceWords[activeReferenceIndex] = {
        ...currentRefWord,
        isCorrect: char.toLowerCase() === expectedChar,
      };
      setReferenceWords(updatedReferenceWords);

      setActiveReferenceIndex(activeReferenceIndex + 1);

      if (activeReferenceIndex + 1 >= referenceWords.length) {
        setActiveReferenceIndex(0);
        setLearnedVerseReference(true);
        setActiveIndex(0);
      }

      return;
    }

    if (activeIndex >= allWords.length)
      return;

    const currentWord = allWords[activeIndex];
    const expectedChar = currentWord.word.trim().charAt(0).toLowerCase();

    if (char.toLowerCase() !== expectedChar) {
      Haptics.notificationAsync(Haptics.NotificationFeedbackType.Error);
    }

    const updatedWords = [...allWords];
    updatedWords[activeIndex] = { ...currentWord, isCorrect: char.toLowerCase() === expectedChar };
    setAllWords(updatedWords);

    setActiveIndex(activeIndex + 1);

    if (activeIndex + 1 >= allWords.length) {
      const currentAccuracy = computeStageAccuracy(updatedWords, referenceWords, bookWords);
      setAccuracy(currentAccuracy);
      const newStageAccuracies = [...stageAccuracies];
      newStageAccuracies[currentStage - 1] = currentAccuracy;
      setStageAccuracies(newStageAccuracies);

      setAccuracyModalVisible(true);
    }
  };

  const keyboardPressRef = useRef(handleKeyboardPress);
  keyboardPressRef.current = handleKeyboardPress;
  const onKeyboardPress = useCallback((char: string) => {
    keyboardPressRef.current(char);
  }, []);

  const retryStage = () => {
    const hiddenSet = new Set(stageHiddenIndeces[currentStage - 1] ?? []);
    setAllWords(
      firstStageWords.map((word, index) => ({
        ...word,
        isHint: !hiddenSet.has(index),
        isCorrect: null,
      }))
    );
    resetReferenceForStage(currentStage);
    setActiveIndex(0);
    setAccuracyModalVisible(false);
  };

  const computeStageAccuracy = (words: Word[], refWords: Word[], bookWordsArray: Word[]) => {
    let allWordsToCheck = [...words];

    if (typeOutReference) {
      if (learnedBook) {
        allWordsToCheck = [...bookWordsArray, ...allWordsToCheck];
      }
      if (learnedVerseReference) {
        allWordsToCheck = [...refWords, ...allWordsToCheck];
      }
    }

    const total = allWordsToCheck.length;
    const correct = allWordsToCheck.filter(w => w.isCorrect === true).length;

    return total > 0 ? Math.floor((correct / total) * 100) : 0;
  };

  const nextStage = async () => {
    setAccuracyModalVisible(false);

    if (currentStage >= TOTAL_STAGES) {
      const sessionAccuracy = Math.floor(
        stageAccuracies.reduce((a, b) => a + b, 0) / Math.max(stageAccuracies.length, 1)
      );
      setAccuracy(sessionAccuracy);

      try {
        await completePractice(userPassage, dateOpened, stageAccuracies);
      } catch (error) {
        console.error('Failed to save practice:', error);
        showToast({ type: 'danger', title: 'Failed to save practice' });
      }

      setSummaryModalVisible(true);
      return;
    }

    const next = currentStage + 1;
    resetReferenceForStage(next);
    setCurrentStage(next);
    setActiveIndex(0);
  };

  useEffect(() => {
    if (!firstStageWords.length || !stageHiddenIndeces.length) return;

    const hiddenSet = new Set(stageHiddenIndeces[currentStage - 1] ?? []);

    setAllWords(
      firstStageWords.map((word, index) => ({
        ...word,
        isHint: !hiddenSet.has(index),
        isCorrect: null,
      }))
    );

    setActiveIndex(0);
  }, [currentStage, stageHiddenIndeces, firstStageWords]);

  useLayoutEffect(() => {
    navigation.setOptions({
      title: typeOutReference ? '' : readableReference,
      headerRight: () => (
        <TouchableOpacity onPress={retryStage} activeOpacity={0.7}>
          <Ionicons name="refresh" size={24} color={theme.colors.onBackground} />
        </TouchableOpacity>
      ),
    });
  }, [navigation, typeOutReference, readableReference, currentStage, stageHiddenIndeces, firstStageWords, theme]);

  const keyBackground = theme.colors.elevation;
  const keyText = theme.colors.onBackground;

  return (
    <>
      <View style={{ flex: 1, backgroundColor: theme.colors.background }}>
        <ScrollView
          style={{ flex: 1 }}
          contentContainerStyle={{ padding: 20, paddingBottom: 100 }}
        >
          <View style={{ height: 5, width: '100%', flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between', marginBottom: 20 }}>
            {[...Array(TOTAL_STAGES)].map((_, index) => (
              <View key={index} style={{ width: '24%', height: 5, borderRadius: 1, backgroundColor: index === currentStage - 1 ? theme.colors.primary : theme.colors.elevation2 }} />
            ))}
          </View>

          <View style={{ width: '100%', borderRadius: 8, marginTop: 20 }}>
            {typeOutReference ? (
              <Text style={{ fontSize: 18, lineHeight: 18, marginBottom: 12, color: 'transparent' }}>
                {renderReference()}
              </Text>
            ) : (
              <Text style={{ fontSize: 18, lineHeight: 18, marginBottom: 12, color: theme.colors.onBackground }}>
                {book} {chapter}:{readableReferenceVerses}
              </Text>
            )}

            <Text style={{ fontSize: 18, color: theme.colors.verseHint, lineHeight: 24 }}>
              {allWords.map((w) => (
                w.isCorrect === null ? (
                  w.isHint ? (
                    <Text key={w.id} style={{ color: theme.colors.verseHint }}>{w.word}</Text>
                  ) : (
                    <Text key={w.id} style={{ color: theme.colors.background }}>{w.word}</Text>
                  )
                ) : (
                  w.isCorrect ? (
                    <Text key={w.id} style={{ color: theme.colors.onBackground }}>{w.word}</Text>
                  ) : (
                    <Text key={w.id} style={{ color: theme.colors.notification }}>{w.word}</Text>
                  )
                )
              ))}
            </Text>
          </View>
        </ScrollView>

        <View style={{ position: 'absolute', bottom: 0, width: '100%', justifyContent: 'center', alignItems: 'center', paddingBottom: 40 }}>
          <View style={{ justifyContent: 'center', alignItems: 'center', flexDirection: 'row' }}>
            {NUMBER_KEYS.map((char) => (
              <KeyboardButton key={char} char={char} height={40} onPress={onKeyboardPress} backgroundColor={keyBackground} textColor={keyText} />
            ))}
          </View>
          {LETTER_ROWS.map((row) => (
            <View key={row.join('')} style={{ justifyContent: 'center', alignItems: 'center', flexDirection: 'row' }}>
              {row.map((char) => (
                <KeyboardButton key={char} char={char} height={50} onPress={onKeyboardPress} backgroundColor={keyBackground} textColor={keyText} />
              ))}
            </View>
          ))}
        </View>
      </View>

      {loading && (
        <View style={{ position: 'absolute', width: '100%', height: '100%', justifyContent: 'center', alignItems: 'center', backgroundColor: theme.colors.background }}>
          <ActivityIndicator size="large" color={theme.colors.primary} />
        </View>
      )}

      <Modal
        visible={accuracyModalVisible}
        transparent={true}
        animationType="fade"
        onRequestClose={() => {}}>
        <View style={{ flex: 1, backgroundColor: 'rgba(0, 0, 0, 0.5)', justifyContent: 'center', alignItems: 'center' }}>
          <View style={{ width: '80%', alignSelf: 'center', alignItems: 'center', backgroundColor: theme.colors.background2, borderRadius: 16, padding: 20, borderWidth: .5, borderColor: theme.colors.onBackgroundSoft }}>
            <Text style={{ ...styles.h2, marginBottom: 10, marginTop: 20 }}>Accuracy: {accuracy}%</Text>
            {accuracy >= 90 ? (
              currentStage >= TOTAL_STAGES ? (
                <Text style={{ ...styles.p2, textAlign: 'center' }}>Good job! Go ahead and finish the session.</Text>
              ) : (
                <Text style={{ ...styles.p2, textAlign: 'center' }}>Good job! Go ahead and continue to the next stage.</Text>
              )
            ) : (
              accuracy >= 75 ? (
                <Text style={{ ...styles.p2, textAlign: 'center' }}>Nice job, but you can get a better score. Try again!</Text>
              ) : (
                <Text style={{ ...styles.p2, textAlign: 'center' }}>It's recommended that you retry this stage.</Text>
              )
            )}
            <View style={{ flexDirection: 'row', marginTop: 20 }}>
              <TouchableOpacity style={{ height: 150, width: '48%', borderWidth: 1, borderColor: theme.colors.onBackgroundSoft, borderRadius: 8, justifyContent: 'center', alignItems: 'center', margin: 10 }}
                onPress={retryStage}>
                <Ionicons name="refresh-outline" size={48} color={theme.colors.onBackgroundSoft} />
                <Text style={{ ...styles.p2, textAlign: 'center' }}>Retry</Text>
              </TouchableOpacity>
              <TouchableOpacity style={{ height: 150, width: '48%', borderWidth: 1, borderColor: theme.colors.onBackgroundSoft, borderRadius: 8, justifyContent: 'center', alignItems: 'center', margin: 10 }}
                onPress={nextStage}>
                <Ionicons name="arrow-forward-outline" size={48} color={theme.colors.onBackgroundSoft} />
                <Text style={{ ...styles.p2, textAlign: 'center' }}>
                  {currentStage >= TOTAL_STAGES ? 'Finish' : 'Next Stage'}
                </Text>
              </TouchableOpacity>
            </View>
          </View>
        </View>
      </Modal>

      <Modal
        visible={summaryModalVisible}
        transparent={true}
        animationType="fade"
        onRequestClose={() => {}}>
        <View style={{ flex: 1, backgroundColor: 'rgba(0, 0, 0, 0.5)', justifyContent: 'center', alignItems: 'center' }}>
          <View style={{ backgroundColor: theme.colors.background2, borderRadius: 12, width: '90%', maxWidth: 400, maxHeight: Dimensions.get('window').height * 0.8,
                         padding: 20, justifyContent: 'space-between', borderWidth: .5, borderColor: theme.colors.onBackgroundSoft }}>

            <View style={{ alignItems: 'center', marginBottom: 32 }}>
              <Text style={{ ...styles.p2, textAlign: 'center', color: theme.colors.onBackgroundSoft }}>
                You have practiced
              </Text>
              <Text style={{ ...styles.h2, textAlign: 'center', fontSize: 20, color: theme.colors.primary, marginTop: 4 }}>
                {readableReference}
              </Text>
            </View>

            <View style={{ width: '100%', marginBottom: 24 }}>
              <View style={{ paddingVertical: 20, paddingHorizontal: 4 }}>
                <View style={{ flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' }}>
                  <Text style={{ color: theme.colors.onBackgroundSoft, fontSize: 22, fontFamily: 'Inter' }}>
                    Accuracy
                  </Text>
                  <Text style={{ color: theme.colors.onBackground, fontSize: 24, fontWeight: '600', fontFamily: 'Inter' }}>
                    {accuracy}%
                  </Text>
                </View>
              </View>
            </View>

            <TouchableOpacity
              style={{ ...styles.elevationButton, backgroundColor: theme.colors.primary, marginTop: 16 }}
              onPress={() => {
                setSummaryModalVisible(false);
                navigation.goBack();
              }}
              activeOpacity={0.7}
            >
              <Text style={{ ...styles.p2, color: theme.colors.white }}>
                Done
              </Text>
            </TouchableOpacity>
          </View>
        </View>
      </Modal>
    </>
  );
}

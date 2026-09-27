import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp, NativeStackScreenProps } from "@react-navigation/native-stack";
import React, { useCallback, useEffect, useMemo } from "react";
import { FlatList, StyleSheet, Text, TouchableOpacity, useWindowDimensions, View } from "react-native";
import { getChaptersForBook } from "../../../types/bibleData";
import { RootStackParamList } from "../../../types/router";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

type Props = NativeStackScreenProps<RootStackParamList, 'chooseChapter'>;

const GAP = 10;
const HORIZONTAL_PADDING = 10;
const MIN_BUTTON = 78;

const ChooseChapterScreen: React.FC<Props> = ({route}: Props) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const { width: windowWidth } = useWindowDimensions();
    const available = windowWidth - HORIZONTAL_PADDING * 2;
    const numColumns = Math.max(3, Math.floor(available / MIN_BUTTON));
    const itemWidth = (available - GAP * (numColumns - 1)) / numColumns;
    const styles = useMemo(() => StyleSheet.create({
        container: {
            padding: HORIZONTAL_PADDING,
        },
        row: {
            gap: GAP,
            marginBottom: GAP,
        },
        chapterButton: {
            height: 80,
            borderRadius: 5,
            borderColor: theme.colors.elevation3,
            borderWidth: 2,
            justifyContent: 'center',
            alignItems: 'center',
        },
        bookNumber: {
            ...globalStyles.p3,
            fontSize: 16,
            fontWeight: 300
        }
    }), [theme, globalStyles.p3]);
    const {book} = route.params;
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();

    const [chapters, setChapters] = React.useState<number[]>([]);

    useEffect(() => {
        setChapters([]);
        const frame = requestAnimationFrame(() => {
            const count = getChaptersForBook(book);
            setChapters(Array.from({ length: count }, (_, i) => i + 1));
        });
        return () => cancelAnimationFrame(frame);
    }, [book]);

    useEffect(() => {
        navigation.setOptions({
            headerTitle: book
        });
    }, [navigation, book]);

    const renderChapter = useCallback(({ item }: { item: number }) => (
        <TouchableOpacity
            style={[styles.chapterButton, { width: itemWidth }]}
            onPress={() => navigation.navigate('read', { book, chapter: item })}
        >
            <Text style={styles.bookNumber}>{item}</Text>
        </TouchableOpacity>
    ), [styles, itemWidth, navigation, book]);

    if (chapters.length === 0) return null;

    return (
        <FlatList
            key={numColumns}
            data={chapters}
            keyExtractor={(chapter) => String(chapter)}
            numColumns={numColumns}
            contentContainerStyle={styles.container}
            columnWrapperStyle={numColumns > 1 ? styles.row : undefined}
            renderItem={renderChapter}
            initialNumToRender={numColumns * 8}
            windowSize={7}
            ListFooterComponent={<View style={{ height: 100 }} />}
        />
    );
}

export default ChooseChapterScreen;

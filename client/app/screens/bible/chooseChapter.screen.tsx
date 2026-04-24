import { NativeStackNavigationProp, NativeStackScreenProps } from "@react-navigation/native-stack";
import { View, Text, StyleSheet, TouchableOpacity } from "react-native";
import { RootStackParamList } from "../../../types/router";
import useAppTheme from "../../theme";
import useGlobalStyles from "../../styles/gobalStyles";
import React, { useMemo } from "react";
import { useNavigation } from "@react-navigation/native";
import { getChaptersForBook } from "../../../types/bibleData";
import { ScrollView } from "react-native-gesture-handler";

type Props = NativeStackScreenProps<RootStackParamList, 'chooseChapter'>;

const ChooseChapterScreen: React.FC<Props> = ({route}: Props) => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        title: {
            marginVertical: 20,
            fontSize: 22,
            fontWeight: 600
        },
        container: {
            display: 'flex',
            flexDirection: 'row',
            flexWrap: 'wrap',
            justifyContent: 'center',
            padding: 10,
            gap: 10
        },
        chapterButton: {
            padding: 15,
            borderRadius: 5,
            marginBottom: 2,
            borderColor: theme.colors.elevation3,
            borderWidth: 2,
            minWidth: 80,
            height: 80,
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center'
        },
        spacer: {
            minWidth: 80,
            height: 0,
            padding: 15,
            marginBottom: 10,
        },
        bookNumber: {
            ...globalStyles.p3,
            fontSize: 16,
            fontWeight: 300
        }
    }), [theme]);
    const styles = useLocalStyles();
    const {book} = route.params;
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    navigation.setOptions({
        headerTitle: book
    });

    const chaptersInBook = getChaptersForBook(book);
    const chapters = useMemo(() => Array.from({ length: chaptersInBook }, (_, i) => i + 1), [chaptersInBook]);

    return (
        <ScrollView contentContainerStyle={styles.container}>
            {chapters.map((c) => (
                <TouchableOpacity style={styles.chapterButton} onPress={() => {
                    navigation.navigate('read', {book, chapter: c})
                }}>
                    <Text style={styles.bookNumber}>{c}</Text>
                </TouchableOpacity>
            ))}
            {Array.from({ length: 5 }).map((_, i) => (
                <View key={`spacer-${i}`} style={styles.spacer} />
            ))}
        </ScrollView>
    )
}

export default ChooseChapterScreen;
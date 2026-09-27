import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp, NativeStackScreenProps } from "@react-navigation/native-stack";
import React, { useEffect, useMemo } from "react";
import { StyleSheet, Text, TouchableOpacity, View } from "react-native";
import { ScrollView } from "react-native-gesture-handler";
import { getChaptersForBook } from "../../../types/bibleData";
import { RootStackParamList } from "../../../types/router";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";

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
            minWidth: 60,
            flexGrow: 1,
            minHeight: 80,
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

    return (
        chapters.length > 0 && (
            <ScrollView contentContainerStyle={styles.container}>
                {chapters.map((c) => (
                    <TouchableOpacity key={c} style={styles.chapterButton} onPress={() => {
                        navigation.navigate('read', {book, chapter: c})
                    }}>
                        <Text style={styles.bookNumber}>{c}</Text>
                    </TouchableOpacity>
                ))}
                {Array.from({ length: 5 }).map((_, i) => (
                    <View key={`spacer-${i}`} style={styles.spacer} />
                ))}
                <View style={{height: 100}} />
            </ScrollView>
        )
    );
}

export default ChooseChapterScreen;
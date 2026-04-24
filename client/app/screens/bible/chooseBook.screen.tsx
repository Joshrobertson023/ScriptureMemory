import { FlatList, StyleSheet, View, Text, TouchableOpacity, SectionList } from "react-native";
import useGlobalStyles from "../../styles/gobalStyles";
import useAppTheme from "../../theme";
import { useContext, useMemo } from "react";
import { ArrowRight } from "lucide-react-native";
import { NavigationContext, useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";
import { newTestamentBooks, oldTestamentBooks } from "../../../types/bibleData";

interface BookProps {
    book: string;
}

const Book = ({book}: BookProps) => {
    const navigation = useNavigation<NativeStackNavigationProp<RootStackParamList>>();
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        row: {
            display: 'flex',
            flexDirection: 'row',
            justifyContent: 'space-between',
            alignItems: 'stretch',
            paddingVertical: 15,
            width: '100%'
        },
        book: {
            fontSize: 18,
            fontWeight: 400
        }
    }), [theme]);
    const styles = useLocalStyles();

    return (
        <TouchableOpacity style={styles.row} onPress={() => navigation.navigate('chooseChapter', {book})}>
            <Text style={[globalStyles.p3, styles.book]}>
                {book}
            </Text>
            <ArrowRight size={24} color={theme.colors.onBackgroundSoft} />
        </TouchableOpacity>
    )
}  

const sections = [
    { title: 'Old Testament', data: oldTestamentBooks.map(b => b.name) },
    { title: 'New Testament', data: newTestamentBooks.map(b => b.name) },
];

const ChooseBookScreen = () => {
    const theme = useAppTheme();
    const globalStyles = useGlobalStyles();
    const useLocalStyles = () => useMemo(() => StyleSheet.create({
        title: {
            marginVertical: 20,
            fontSize: 22,
            fontWeight: 600
        },
        container: {
            paddingHorizontal: 15,
            paddingTop: 40
        }
    }), [theme]);
    const styles = useLocalStyles();
    
    return (
        <View style={styles.container}>
            <SectionList
                sections={sections}
                keyExtractor={(item) => item}
                renderItem={({ item }) => <Book book={item} />}
                renderSectionHeader={({ section: { title } }) => (
                    <Text style={[globalStyles.p2, styles.title]}>{title}</Text>
                )}
                contentContainerStyle={{ paddingBottom: 20 }}
            />
        </View>
    )
}

export default ChooseBookScreen;
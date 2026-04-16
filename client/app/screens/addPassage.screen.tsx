import { Text, TextInput, View, useWindowDimensions } from "react-native"
import useGlobalStyles from "../styles/gobalStyles"
import { useState } from "react";
import useAppTheme from "../theme";
import { Search } from "lucide-react-native";
import { FlatList } from "react-native-gesture-handler";
import { Passage } from "../../types/passages/passage";
import { searchPassage } from "../api/verses.api";
import { Snackbar } from "react-native-snackbar";
import * as Clipboard from 'expo-clipboard';
import { UserPassage } from "../../types/passages/userPassage";
import AddPassage from "../components/passage/addPassage";

export const AddPassageScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const [search, setSearch] = useState('');
    const [searchResults, setSearchResults] = useState<Passage[]>([]);
    const [loadingSearch, setLoadingSearch] = useState(false);

    const handleSearch = async () => {
        if (search.trim() === '')
            return;

        setLoadingSearch(true);
        try {
            setSearchResults(await searchPassage(search));
        } catch (error) {
            console.error('error searching');
            Snackbar.show({
                text: 'We encountered an error',
                duration: Snackbar.LENGTH_SHORT,
                action: {
                    text: 'COPY ERROR',
                    textColor: theme.colors.onBackground,
                    onPress: async () => {await Clipboard.setStringAsync(String(error))}
                }
            })
        } finally {
            setLoadingSearch(false);
        }
    }

    return (
        <View style={{...styles.screen, paddingTop: 40}}>
            <View style={{display: 'flex', flexDirection: 'row', backgroundColor: theme.colors.elevation, padding: 10}}>
                <Search size={28} color={theme.colors.onBackground} />
                <TextInput
                    style={[styles.search, { flex: 0, width: '100%', marginLeft: 25}]}
                    value={search}
                    onChangeText={(text) => setSearch(text)}
                    placeholder="Search the Bible"
                    onSubmitEditing={handleSearch} // Triggers when Enter is pressed
                    returnKeyType="search"
                />
            </View>

            <FlatList
                data={searchResults}
                keyExtractor={(item) => item.reference.readableReference}
                renderItem={({item}) => <AddPassage passage={item} />} />
        </View>
    )
}
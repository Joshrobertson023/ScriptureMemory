import { Text, TextInput, View, useWindowDimensions } from "react-native"
import useGlobalStyles from "../styles/gobalStyles"
import { useState } from "react";
import useAppTheme from "../theme";
import { Search } from "lucide-react-native";

export const AddPassageScreen = () => {
    const styles = useGlobalStyles();
    const theme = useAppTheme();
    const [search, setSearch] = useState('');

    const handleSearch = () => {

    }

    return (
        <View style={{...styles.screen, paddingTop: 40}}>
            <View style={{display: 'flex', flexDirection: 'row', backgroundColor: theme.colors.elevation, padding: 10}}>
                <Search size={28} color={theme.colors.onBackground} />
                <TextInput
                    style={[styles.input, { flex: 0, width: '100%', marginLeft: 25}]}
                    value={search}
                    onChangeText={(text) => setSearch(text)}
                    placeholder="Search the Bible"
                    onSubmitEditing={handleSearch} // Triggers when Enter is pressed
                    returnKeyType="search"
                />
            </View>
        </View>
    )
}
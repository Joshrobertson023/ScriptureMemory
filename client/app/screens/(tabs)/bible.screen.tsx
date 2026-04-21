import { Button, Text, View } from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import useStyles from '../../styles/gobalStyles';

export const BibleScreen = () => {
    const styles = useStyles();
    
    return (
        <SafeAreaView style={styles.screen}>
            <Text style={styles.p1}>Bible</Text>
        </SafeAreaView>
    )
}
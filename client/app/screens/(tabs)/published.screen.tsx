import { Button, Text, View } from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import useStyles from '../../styles/gobalStyles';
import * as SystemUI from 'expo-system-ui';

SystemUI.setBackgroundColorAsync('#181818')

export const PublishedScreen = () => {
    const styles = useStyles();
    
    return (
        <SafeAreaView style={styles.screen}>
            <Text style={styles.p1}>Published</Text>
        </SafeAreaView>
    )
}
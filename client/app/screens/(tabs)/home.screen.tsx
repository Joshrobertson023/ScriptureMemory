import { Button, Text, View } from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import useStyles from '../../styles/gobalStyles';
import * as SystemUI from 'expo-system-ui';
import { VerseOfDayHomeCard } from "../../components/home/vod";

SystemUI.setBackgroundColorAsync('#181818')

export const HomeScreen = () => {
    const styles = useStyles();
    
    return (
        <View style={styles.screen}>
            <VerseOfDayHomeCard/>
            <View style={{display: 'flex', marginTop: 35, width: '100%'}}>
                <Text style={styles.h1}>For You</Text>
            </View>
        </View>
    )
}
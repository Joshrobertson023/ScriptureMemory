import { SafeAreaView } from "react-native-safe-area-context";
import useStyles from "../../styles";
import { Keyboard, TouchableWithoutFeedback } from "react-native";

export default function LoginScreen() {
    const styles = useStyles();

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
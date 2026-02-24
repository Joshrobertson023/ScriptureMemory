import { SafeAreaView } from "react-native-safe-area-context";
import useStyles from "../../styles";
import { Keyboard, TouchableWithoutFeedback } from "react-native";
import { LoginUsernameInput } from "../../components/auth/LoginUsernameInput";
import LoginPasswordInput from "../../components/auth/LoginPasswordInput";

export default function LoginScreen() {
    const styles = useStyles();

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                <LoginUsernameInput />
                <LoginPasswordInput />
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
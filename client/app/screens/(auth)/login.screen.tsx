import { SafeAreaView } from "react-native-safe-area-context";
import useStyles from "../../styles";
import { Keyboard, TouchableWithoutFeedback } from "react-native";
import { LoginUsernameInput } from "../../components/auth/LoginUsernameInput";
import LoginPasswordInput from "../../components/auth/LoginPasswordInput";
import { Button, Text } from "react-native-paper";
import { useState } from "react";

export default function LoginScreen() {
    const styles = useStyles();

    const [loading, setLoading] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');

    const login = () => {
        setLoading(true);

        try {

        } catch (error) {
            setErrorMessage("There was an error logging in. Please try again later.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                <LoginUsernameInput />
                <LoginPasswordInput />
                <Button style={styles.button_filled} onPress={() => {login()}}>Login</Button>
                {errorMessage && <Text style={styles.tinyText}>{errorMessage}</Text>}
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
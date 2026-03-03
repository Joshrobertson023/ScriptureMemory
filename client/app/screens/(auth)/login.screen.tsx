import { SafeAreaView } from "react-native-safe-area-context";
import useStyles from "../../styles";
import { Keyboard, TouchableOpacity, TouchableWithoutFeedback, View } from "react-native";
import { LoginUsernameInput } from "../../components/auth/LoginUsernameInput";
import LoginPasswordInput from "../../components/auth/LoginPasswordInput";
import { Button, Text } from "react-native-paper";
import { useState } from "react";
import Logo from "../../components/temp/logo";
import { useNavigation } from "@react-navigation/native";
import { NativeStackNavigationProp } from "@react-navigation/native-stack";
import { RootStackParamList } from "../../../types/router";

export default function LoginScreen() {
    const styles = useStyles();

    type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
    const nav = useNavigation<NavigationProp>();

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
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()} style={{width: '100%', height: '100%'}}>
                <View>
                    <Logo/>
                    <LoginUsernameInput />
                    <LoginPasswordInput />
                    <TouchableOpacity style={styles.button_filled} onPress={() => {login()}}>
                        <Text style={styles.buttonText_filled}>Login</Text>
                    </TouchableOpacity>
                    {errorMessage && <Text style={styles.tinyText}>{errorMessage}</Text>}
                    <TouchableOpacity style={styles.button_text} onPress={() => nav.navigate('createAccount')}>
                        <Text style={styles.buttonText_outlined}>Create Account</Text>
                    </TouchableOpacity>
                </View>
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
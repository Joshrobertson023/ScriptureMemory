import React, { useState } from 'react';
import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { HelperText, TextInput } from 'react-native-paper';
import { SafeAreaView } from 'react-native-safe-area-context';
import Logo from '../../components/temp/logo';
import { useAppStore } from '../../store';
import useStyles from '../../styles';
import useAppTheme from '../../theme';

import { LoginButton } from '../../components/auth/LoginButton';
import { NameInput } from '../../components/auth/NameInput';

export interface RegisterForm {
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
    errors: Record<string, string>;
}

export default function CreateAccountScreen() {
    const styles = useStyles();
    const theme = useAppTheme();
    const store = useAppStore();

    const [errorMessage, setErrorMessage] = useState('');

    const [form, setForm] = useState<RegisterForm>({
        firstName: '',
        lastName: '',
        username: '',
        email: '',
        password: '',
        confirmPassword: '',
        errors: {} as Record<string, string>
    });
    
    const nextClick = () => {
        Keyboard.dismiss();
    };


    return (
        <SafeAreaView style={styles.container}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                <Logo />
                <Text style={{
                    color: theme.colors.onBackground,
                    fontSize: 24,
                    marginBottom: 16,
                    fontFamily: 'Inter',
                    fontWeight: 900,
                }}>
                    Create an Account
                </Text>
                <NameInput f />
                <LoginButton />
                {errorMessage && <Text style={styles.errorMessage}>{errorMessage}</Text>}
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
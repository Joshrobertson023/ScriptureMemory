import React, { useState } from 'react';
import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import Logo from '../../components/temp/logo';
import useStyles from '../../styles';
import useAppTheme from '../../theme';

import { LoginButton } from '../../components/auth/LoginButton';
import { NameInput } from '../../components/auth/NameInput';
import { UsernameInput } from '../../components/auth/UsernameInput';
import { useFormStore } from '../../stores/form.store';
import { EmailInput } from '../../components/auth/EmailInput';
import { CreatePasswordInput } from '../../components/auth/CreatePasswordInput';
import { Button } from 'react-native-paper';
import { usernameExists, createUser } from '../../api/user.api';

export default function CreateAccountScreen() {
    const styles = useStyles();
    const theme = useAppTheme();

    const [errorMessage, setErrorMessage] = useState('');
    const [loading, setLoading] = useState(false);
    const [doesUsernameExist, setDoesUsernameExist] = useState(false);
    const form = useFormStore(s => s.registerForm);

    const createAccount = async () => {
        setLoading(true);
        if (await usernameExists(form.username) || !form.firstName || !form.lastName 
            || !form.username || !form.email || !form.password || !form.confirmPassword)
            return;
        try {
            
        } catch (error) {
            setErrorMessage("There was a problem creating an account. Please try again.");
        } finally {
            setLoading(false);
        }
    }

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                <Logo />
                <Text style={{
                    color: theme.colors.onBackground,
                    fontSize: 24,
                    marginBottom: 16,
                    fontFamily: 'Inter',
                    fontWeight: 900,
                }}>
                    Create Account
                </Text>
                <NameInput />
                <UsernameInput />
                <LoginButton />
                <EmailInput />
                <CreatePasswordInput />
                <Button style={styles.buttonFilled} onPress={() => createAccount()}>Create Account</Button>
                {errorMessage && <Text style={styles.errorMessage}>{errorMessage}</Text>}
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
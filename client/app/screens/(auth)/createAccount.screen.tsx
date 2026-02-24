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
import { PasswordInput } from '../../components/auth/PasswordInput';

export default function CreateAccountScreen() {
    const styles = useStyles();
    const theme = useAppTheme();

    const errorMessage = useFormStore(s => s.registerForm.errorMessage);

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
                <PasswordInput />
                {errorMessage && <Text style={styles.errorMessage}>{errorMessage}</Text>}
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
import React, { useState } from 'react';
import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { HelperText, TextInput } from 'react-native-paper';
import { SafeAreaView } from 'react-native-safe-area-context';
import Logo from '../../components/temp/logo';
import { useAppStore } from '../../store';
import useStyles from '../../styles';
import useAppTheme from '../../theme';

import { NameInputProps } from "../../components/auth/NameInput";

import { LoginButton } from '../../components/auth/LoginButton';
import { NameInput } from '../../components/auth/NameInput';

interface LoginInfo {
    firstName: string;
    lastName: string;
    username: string;
    password: string;
    confirmPassword: string;
    email: string;
    bibleVersion?: number;
}

export default function CreateAccountScreen() {
  const styles = useStyles();
  const [loginInfo, setLoginInfo] = useState<LoginInfo>();
  const theme = useAppTheme();

 
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
                    <NameInput />
                    <LoginButton />
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
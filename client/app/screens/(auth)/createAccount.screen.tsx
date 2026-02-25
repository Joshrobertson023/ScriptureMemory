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
import { ActivityIndicator, Button } from 'react-native-paper';
import { usernameExists, createUser, loginUser } from '../../api/user.api';
import {
  useQuery,
  useMutation,
  useQueryClient,
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'
import { useUserAuthStore } from '../../stores/userAuth.store';
import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../../_layout';

export default function CreateAccountScreen() {
    const styles = useStyles();
    const theme = useAppTheme();
    type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
    const navigation = useNavigation<NavigationProp>();

    const [errorMessage, setErrorMessage] = useState('');
    const [loading, setLoading] = useState(false);
    const [doesUsernameExist, setDoesUsernameExist] = useState(false);
    const form = useFormStore(s => s.registerForm);
    const setToken = useUserAuthStore(s => s.setToken);

    const queryClient = useQueryClient();

    // -- Mutations ----------------------------------
    const createMutation = useMutation({
        mutationFn: () =>
            createUser(
                form.username,
                form.firstName,
                form.lastName,
                form.email,
                form.password,
                form.bibleVersion
            ),

        onSuccess: async () => {
                await loginMutation.mutateAsync({
                username: form.username,
                password: form.password,
            });
        },
    });

    const loginMutation = useMutation({
        mutationFn: (data: { username: string; password: string; }) => loginUser(data.username, data.password),
        onSuccess: async (data) => {
            if (data.authToken)
                setToken(data.authToken);

            navigation.navigate('(tabs)');
        }   
    })

    // -- Create Account ------------------------------
    const createAccount = async () => {
        setLoading(true);
        if (await usernameExists(form.username) || form.firstName === '' || form.lastName === ''
            || form.username === '' || form.email === '' || form.password === '' || form.confirmPassword === '')
            return;
        
        try {
            console.log(form);
            await createUser(form.username, form.firstName, form.lastName, form.email, form.password, form.bibleVersion);
            await loginMutation.mutateAsync({username: form.username, password: form.password});
        } catch (error) {
            setErrorMessage("There was a problem creating your account. Please try again later.");
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
                <Button style={styles.buttonFilled} onPress={() => createAccount()}>
                    {loading ? (
                        <ActivityIndicator animating={true} color={theme.colors.background}/>
                    ) : (
                        <Text style={styles.tinyText}>Create Account</Text>
                    )}
                </Button>
                {errorMessage && <Text style={styles.errorMessage}>{errorMessage}</Text>}
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
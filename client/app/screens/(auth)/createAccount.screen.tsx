import React, { useCallback, useState } from 'react';
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
import { ActivityIndicator, Button, IconButton, Tooltip } from 'react-native-paper';
import { usernameExists, createUser, loginUser } from '../../api/user.api';
import {
  useQuery,
  useMutation,
  useQueryClient,
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'
import { useUserAuthStore } from '../../stores/userAuth.store';
import { useFocusEffect, useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../../../types/router';

export default function CreateAccountScreen() {
    const styles = useStyles();
    const theme = useAppTheme();
    type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
    const navigation = useNavigation<NavigationProp>();

    const [errorMessage, setErrorMessage] = useState('');
    const [loading, setLoading] = useState(false);
    const form = useFormStore(s => s.registerForm);
    const empty = useFormStore(s => s.registerForm.empty);
    const updateForm = useFormStore(s => s.updateRegister);
    const setToken = useUserAuthStore(s => s.setToken);

    // Set empty errors all to false when opening screen
    useFocusEffect(
        useCallback(() => {
            updateForm({
                empty: {
                    firstName: false,
                    lastName: false,
                    username: false,
                    email: false,
                    password: false,
                    confirmPassword: false,
                }
            })
        }, [])
    )

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

    // Create account
    const createAccount = async () => {
        if (!form.username || !form.email || !form.password || !form.confirmPassword) return;
        if ((!form.firstName && form.lastName) || (form.firstName && !form.lastName)) return;

        setLoading(true);

        try {
            console.log(form);
            await createUser(form.username, form.firstName, form.lastName, form.email, form.password, 0);
            await loginMutation.mutateAsync({username: form.username, password: form.password});
        } catch (error) {
            // Log both error.message and the error object
            console.log('Error object:', error);
            if (error instanceof Error) {
                setErrorMessage(error.message);
                console.log('Error message:', error.message);
            } else {
                setErrorMessage(JSON.stringify(error));
                console.log('Error (non-Error):', error);
            }
        } finally {
            setLoading(false);
        }
    }

    return (
        <SafeAreaView style={styles.screen}>
            <TouchableWithoutFeedback onPress={() => Keyboard.dismiss()}>
                <View>
                    <Text style={{
                        color: theme.colors.onBackground,
                        fontSize: 24,
                        marginBottom: 16,
                        fontFamily: 'Inter',
                        fontWeight: 900,
                    }}>
                        Create Account
                    </Text>
                    <Text style={styles.tinyText}>Optional: enter your name</Text>
                    <Tooltip title="Help people search you">
                        <IconButton icon="question" selected size={16} onPress={() => {}}/>
                    </Tooltip>
                    <NameInput />
                    <UsernameInput />
                    <EmailInput />
                    <CreatePasswordInput />
                    <TouchableOpacity style={styles.buttonFilled} onPress={() => createAccount()}>
                        {loading ? (
                            <ActivityIndicator animating={true} color={theme.colors.background}/>
                        ) : (
                            <Text style={styles.buttonText_filled}>Create Account</Text>
                        )}
                    </TouchableOpacity>
                    {errorMessage && <Text style={styles.errorMessage}>{errorMessage}</Text>}
                    <LoginButton />
                </View>
            </TouchableWithoutFeedback>
        </SafeAreaView>
    )
}
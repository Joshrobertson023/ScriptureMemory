import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { ActivityIndicator, HelperText, TextInput } from 'react-native-paper';
import useStyles from "../../styles";
import { useAppStore } from '../../store';
import { useState } from 'react';
import { RegisterForm } from '../../screens/(auth)/createAccount.screen';

interface NameInputProps {
    form: RegisterForm;
    setForm: (f: RegisterForm) => void;
}

export const NameInput = ({ form, setForm }: NameInputProps) => {
    
    const styles = useStyles();
    const firstName = useAppStore(s => s.loginInfo.firstName);
    const lastName = useAppStore(s => s.loginInfo.lastName);
    const loginInfo = useAppStore(s => s.loginInfo);
    const setLoginInfo = useAppStore(s => s.setLoginInfo);

    return (
        <>
            <TextInput style={styles.input} value={firstName}
                error={firstNameEmpty}
                onChangeText={(text) => {
                    setLoginInfo({...loginInfo, firstName});
                    if (errorMessage.includes('enter all fields')) setErrorMessage('');
                    if (text) setFirstNameEmpty(false);
                }} />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={firstNameEmpty}>
                Enter your first name
            </HelperText>

            <TextInput label="Last Name" mode="outlined" style={styles.input} value={lastName}
                error={lastNameEmpty}
                onChangeText={(text) => {
                    setLoginInfo({...loginInfo, lastName});
                    if (errorMessage) setErrorMessage('');
                    if (text) setLastNameEmpty(false);
                }} />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={lastNameEmpty}>
                Enter your last name
            </HelperText>
        </>
    )
}
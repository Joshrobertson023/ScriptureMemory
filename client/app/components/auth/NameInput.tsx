import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { ActivityIndicator, HelperText, TextInput } from 'react-native-paper';
import useStyles from "../../styles";
import { useAppStore } from '../../store';
import { useState } from 'react';

interface NameInputProps {
    errorMessage: string;
    setErrorMessage: (message: string) => void;
}

export const NameInput = ({ errorMessage, setErrorMessage }: NameInputProps) => {
    const styles = useStyles();
    const firstName = useAppStore(s => s.loginInfo.firstName);
    const lastName = useAppStore(s => s.loginInfo.lastName);
    const loginInfo = useAppStore(s => s.loginInfo);
    const setLoginInfo = useAppStore(s => s.setLoginInfo);

    const [firstNameEmpty, setFirstNameEmpty] = useState(false);
    const [lastNameEmpty, setLastNameEmpty] = useState(false);

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
                    if (errorMessage.includes('enter all fields')) setErrorMessage('');
                    if (text) setLastNameEmpty(false);
                }} />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={lastNameEmpty}>
                Enter your last name
            </HelperText>
        </>
    )
}
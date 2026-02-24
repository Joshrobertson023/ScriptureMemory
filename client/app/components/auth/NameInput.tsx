import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import { ActivityIndicator, HelperText, TextInput } from 'react-native-paper';
import useStyles from "../../styles";
import { useAppStore } from '../../store';
import { useState } from 'react';
import { useFormStore } from '../../stores/form.store';


export const NameInput = () => {
    
    const styles = useStyles();
    const firstName = useFormStore(s => s.registerForm.firstName);
    const lastName = useFormStore(s => s.registerForm.lastName);
    const updateForm = useFormStore(s => s.updateRegister);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);

    const [firstNameEmpty, setFirstNameEmpty] = useState(false);
    const [lastNameEmpty, setLastNameEmpty] = useState(false);

    return (
        <>
            <TextInput style={styles.input} value={firstName}
                error={firstNameEmpty}
                onChangeText={(text) => {
                    updateForm({firstName: text});
                    if (errorMessage) updateForm({errorMessage: ''})
                    if (text) setFirstNameEmpty(false);
                    else setFirstNameEmpty(true);
                }} />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={firstNameEmpty}>
                Enter your first name
            </HelperText>

            <TextInput label="Last Name" mode="outlined" style={styles.input} value={lastName}
                error={lastNameEmpty}
                onChangeText={(text) => {
                    updateForm({lastName: text})
                    if (errorMessage) updateForm({errorMessage: ''})
                    if (text) setLastNameEmpty(false);
                    else setLastNameEmpty(true);
                }} />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={lastNameEmpty}>
                Enter your last name
            </HelperText>
        </>
    )
}
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
    const form = useFormStore(s => s.registerForm);

    const firstNameEmpty = useFormStore(s => s.registerForm.empty.firstName);
    const lastNameEmpty = useFormStore(s => s.registerForm.empty.lastName);

    return (
        <View>
            <TextInput style={styles.input} 
                value={firstName}
                mode='outlined'
                outlineStyle={{borderRadius: 40}}
                label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}>First Name</Text>}
                error={firstNameEmpty}
                onChangeText={(text) => {
                    updateForm({firstName: text});
                    if (errorMessage) updateForm({errorMessage: ''})
                    // if (text) {
                    //     updateForm({
                    //         empty: {
                    //             ...form.empty,
                    //             firstName: false
                    //         }
                    //     })
                    // }
                    // else {
                    //     updateForm({
                    //         empty: {
                    //             ...form.empty,
                    //             firstName: true
                    //         }
                    //     })
                    // }
                }} 
                maxLength={30}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={firstNameEmpty}>
                Enter your first name
            </HelperText>

            <TextInput mode="outlined" style={{...styles.input, marginTop: -10}} value={lastName}
                outlineStyle={{borderRadius: 40}}
                label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}> Last Name</Text>}
                error={lastNameEmpty}
                onChangeText={(text) => {
                    updateForm({lastName: text})
                    if (errorMessage) updateForm({errorMessage: ''})
                    // if (text) {
                    //     updateForm({
                    //         empty: {
                    //             ...form.empty,
                    //             lastName: false
                    //         }
                    //     })
                    // }
                    // else {
                    //     updateForm({
                    //         empty: {
                    //             ...form.empty,
                    //             lastName: true
                    //         }
                    //     })
                    // }
                }} 
                maxLength={30}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={lastNameEmpty}>
                Enter your last name
            </HelperText>
        </View>
    )
}
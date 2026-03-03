import { HelperText, TextInput } from "react-native-paper"
import { useFormStore } from "../../stores/form.store"
import { useState } from "react";
import useStyles from "../../styles";
import { View, Text } from "react-native";

export const EmailInput = () => {
    const email = useFormStore(s => s.registerForm.email);
    const updateForm = useFormStore(s => s.updateRegister);
    const form = useFormStore(s => s.registerForm);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);
    const [emailEmpty, setEmailEmpty] = useState(false);

    const styles = useStyles();

    return (
        <View>
            <TextInput style={{...styles.input}} value={email} error={emailEmpty}
                mode="outlined"
                outlineStyle={{borderRadius: 40}}
                label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}>Email</Text>}
                onChangeText={(text) => {
                    updateForm({email: text});
                    if (errorMessage) updateForm({errorMessage: ''});
                    if (text) {
                        updateForm({
                            empty: {
                                ...form.empty,
                                email: false,
                            }
                        })
                    }
                    else {
                        updateForm({
                            empty: {
                                ...form.empty,
                                email: true,
                            }
                        })
                    }
                }} 
                maxLength={40}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={emailEmpty}>
                Enter your email
            </HelperText>
        </View>
    )
}
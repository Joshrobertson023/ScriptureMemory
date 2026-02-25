import { HelperText, TextInput } from "react-native-paper"
import { useFormStore } from "../../stores/form.store"
import { useState } from "react";
import useStyles from "../../styles";

export const EmailInput = () => {
    const email = useFormStore(s => s.registerForm.email);
    const updateForm = useFormStore(s => s.updateRegister);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);
    const [emailEmpty, setEmailEmpty] = useState(false);

    const styles = useStyles();

    return (
        <>
            <TextInput style={{...styles.input}} value={email} error={emailEmpty}
                onChangeText={(text) => {
                    updateForm({email: email});
                    if (errorMessage) updateForm({errorMessage: ''});
                    if (text) setEmailEmpty(false);
                    else setEmailEmpty(true);
                }} 
                maxLength={40}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={emailEmpty}>
                Enter your email
            </HelperText>
        </>
    )
}
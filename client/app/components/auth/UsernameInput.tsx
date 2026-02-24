import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useState } from "react";
import { useAppStore } from "../../store";
import { useFormStore } from "../../stores/form.store";


export const UsernameInput = () => {
    const styles = useStyles();
    const [usernameEmpty, setUsernameEmtpy] = useState(false);
    const username = useFormStore(s => s.registerForm.username);
    const updateForm = useFormStore(s => s.updateRegister);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);

    return (
        <>
            <TextInput style={{...styles.input}} error={usernameEmpty} value={username} 
                onChangeText={ (text) => {
                    updateForm({username: username});
                    if (errorMessage) updateForm({errorMessage: ''})
                    if (text) setUsernameEmtpy(false);
                    else setUsernameEmtpy(true);
                }
                } />

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameEmpty}>
                Create a username
            </HelperText>
        </>
    )
}
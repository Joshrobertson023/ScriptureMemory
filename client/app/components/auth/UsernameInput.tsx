import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useState } from "react";
import { useAppStore } from "../../store";
import { useFormStore } from "../../stores/form.store";
import { usernameExists } from "../../api/user.api";


export const UsernameInput = () => {
    const styles = useStyles();
    const [usernameEmpty, setUsernameEmtpy] = useState(false);
    const username = useFormStore(s => s.registerForm.username);
    const updateForm = useFormStore(s => s.updateRegister);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);
    const [doesUsernameExist, setDoesUsernameExist] = useState(false);

    return (
        <>
            <TextInput style={{...styles.input}} error={usernameEmpty || doesUsernameExist} value={username} 
                onChangeText={ async (text) => {
                    if (text) 
                        setUsernameEmtpy(false);
                    else 
                        setUsernameEmtpy(true);
                    if (errorMessage) 
                        updateForm({errorMessage: ''})
                    if (await usernameExists(text)) 
                        setDoesUsernameExist(true);
                    updateForm({username: text});
                }} 
                maxLength={30}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameEmpty}>
                Create a username
            </HelperText>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={doesUsernameExist}>
                Username already exists
            </HelperText>
        </>
    )
}
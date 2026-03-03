import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useState } from "react";
import { useAppStore } from "../../store";
import { useFormStore } from "../../stores/form.store";
import { usernameExists } from "../../api/user.api";
import { View, Text } from "react-native";

export const UsernameInput = () => {
    const styles = useStyles();
    const username = useFormStore(s => s.registerForm.username);
    const updateForm = useFormStore(s => s.updateRegister);
    const form = useFormStore(s => s.registerForm);
    const errorMessage = useFormStore(s => s.registerForm.errorMessage);
    const usernameEmpty = useFormStore(s => s.registerForm.empty.username);
    const [doesUsernameExist, setDoesUsernameExist] = useState(false);

    return (
        <View>
            <TextInput style={{...styles.input}} error={usernameEmpty || doesUsernameExist} value={username} 
                mode="outlined"
                outlineStyle={{borderRadius: 40}}
                label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}>Create Username</Text>}
                onChangeText={ async (text) => {
                    if (text) {
                        updateForm({
                            empty: {
                                ...form.empty,
                                username: false,
                            }
                        })
                    }
                    else {
                        updateForm({
                            empty: {
                                ...form.empty,
                                username: true,
                            }
                        })
                    }
                    if (errorMessage) 
                        updateForm({errorMessage: ''})
                    updateForm({username: text});
                    if (await usernameExists(text)) 
                        setDoesUsernameExist(true);
                }} 
                maxLength={30}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameEmpty}>
                Create a username
            </HelperText>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={doesUsernameExist}>
                Username already exists
            </HelperText>
        </View>
    )
}
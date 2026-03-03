import { HelperText, TextInput } from "react-native-paper";
import { useFormStore } from "../../stores/form.store";
import useStyles from "../../styles"
import { useState } from "react";
import { View, Text } from "react-native";

export const LoginUsernameInput = () => {
    const styles = useStyles();
    const username = useFormStore(s => s.loginForm.username);
    const updateForm = useFormStore(s => s.updateLogin);
    const errorMessage = useFormStore(s => s.loginForm.errorMessage);
    const [usernameEmpty, setUsernameEmpty] = useState(false);
    const usernameIncorrect = useFormStore(s => s.loginForm.usernameIncorrect);

    return (
        <View>
            <TextInput value={username} error={usernameEmpty || usernameIncorrect}
                onChangeText={(text) => {
                    updateForm({username: text});
                    if (errorMessage) updateForm({errorMessage: ''})
                    if (text) setUsernameEmpty(false);
                    else setUsernameEmpty(true);
                }} 
                maxLength={30}
                mode={'outlined'}
                style={styles.input}
                label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}>Username</Text>}
                outlineStyle={{borderRadius: 40}}/>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameEmpty}>
                Enter your username
            </HelperText>

            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={usernameIncorrect}>
                Incorrect username
            </HelperText>
        </View>
    )
}
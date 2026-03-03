import { HelperText, TextInput } from "react-native-paper";
import useStyles from "../../styles";
import { useFormStore } from "../../stores/form.store";
import { useState } from "react";
import { View, Text } from "react-native";

export default function LoginPasswordInput() {
    const styles = useStyles();
    const password = useFormStore(s => s.loginForm.password);
    const updateForm = useFormStore(s => s.updateLogin);
    const errorMessage = useFormStore(s => s.loginForm.errorMessage);
    const passwordIncorrect = useFormStore(s => s.loginForm.passwordIncorrect);
    const [showPassword, setShowPassword] = useState(false);
    const [passwordEmpty, setPasswordEmpty] = useState(false);

    return (
        <View>
            <TextInput keyboardType="default"
                        autoCapitalize="none"
                        autoCorrect={false}
                        autoComplete="password"
                        textContentType="password"
                        secureTextEntry={!showPassword}
                        error={passwordEmpty}
                        label={<Text style={{ left: -10, marginLeft: 20, fontSize: 14 }}>Password</Text>}
                        mode="outlined" style={styles.input} value={password}
                        right={<TextInput.Icon icon={showPassword ? 'eye-off' : 'eye'} onPress={() => setShowPassword((prev) => !prev)} />}
                        onChangeText={(text) => {
                            const trimmed = text.trim();
                            updateForm({password: trimmed})
                            if (text) setPasswordEmpty(false);
                        }} 
                        maxLength={30}
                        outlineStyle={{borderRadius: 40}}/>
            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={passwordEmpty}>
                Enter your password
            </HelperText>
            <HelperText style={{textAlign: 'left', width: '100%', marginTop: -15, marginBottom: -5, height: 25}} type="error" visible={passwordIncorrect}>
                Incorrect password
            </HelperText>
        </View>
    )
}
import { Alert, Keyboard, Text, TouchableOpacity, TouchableWithoutFeedback, View } from 'react-native';
import useStyles from "../../styles";
import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../../../types/router';

export const LoginButton = () => {
    const styles = useStyles();
    type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
    const nav = useNavigation<NavigationProp>();
    
    return (
        <View>
            <Text style={{...styles.tinyText, marginTop: 20}}>Already have an account?</Text>
            <TouchableOpacity onPress={() => nav.navigate('login')} style={styles.button_text}>
                <Text style={{...styles.buttonText_outlined, textDecorationLine: 'underline'}}>Login</Text>
            </TouchableOpacity>
        </View>
    )
}
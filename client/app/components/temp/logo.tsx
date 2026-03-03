import React from 'react';
import { Image, Text, View } from 'react-native';
import useAppTheme from '../../theme';
import { Lora_400Regular, Lora_700Bold } from '@expo-google-fonts/lora';

const Logo = () => {
    const theme = useAppTheme();

    return (
        <View>
            <View style={{
                flexDirection: 'row',
                alignItems: 'center',
                // justifyContent: 'center'
            }}>
                <Image style={{width: 60, height: 70, marginTop: 5}} source={require('../../../assets/images/LogoIcon.png')}/>
                <View style={{
                    flexDirection: 'column',
                    marginLeft: 10,
                }}>
                    <Text style={{ 
                        fontSize: 38, color: theme.colors.onBackground,
                        textAlign: 'left', width: '100%', fontFamily: 'Lora'
                    }}>Scripture</Text>
                    <Text style={{ 
                        fontSize: 38, color: theme.colors.onBackground,
                        textAlign: 'left', width: '100%', fontFamily: 'Lora',
                        marginTop: -10,
                    }}>Memory</Text>
                </View>
            </View>

        </View>
    )
}

export default Logo;
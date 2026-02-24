import React from 'react';
import { Text } from 'react-native';
import useAppTheme from '../../theme';

const Logo = () => {
    const theme = useAppTheme();

    return (
        <Text style={{ 
            fontSize: 24, color: theme.colors.secondary,
            textAlign: 'center', marginBottom: 0, width: '100%', top: 50, position: 'relative', zIndex: 1, fontFamily: 'Noto Serif bold'
         }}>Scripture Memory</Text>
    )
}

export default Logo;
import { View, Text } from "react-native"
import useAppTheme from "../../theme"
import React from "react";
import Ionicons from '@expo/vector-icons/Ionicons';
import useGlobalStyles from "../../styles/gobalStyles";

export const VerseOfDayHomeCard = () => {
    const theme = useAppTheme();
    const style = useGlobalStyles();

    const vodReference = 'John 3:16'
    const vodText = 'For God so loved the world, that he gave his only begotten Son, that whosoever believeth in him should not perish, but have everlasting life.'

    return (
        <View style={{width: '100%', backgroundColor: theme.colors.primary, borderRadius: 15, 
            flexDirection: 'row', justifyContent: 'space-between', padding: 20}}>
            <View style={{width: '85%', justifyContent: 'space-between'}}>
                <View>
                    <Text style={style.p4}>Verse of the Day</Text>
                    <Text style={style.h1}>{vodReference}</Text>
                </View>
                <View style={{marginTop: 50}}>
                    <Text style={{...style.p2, fontFamily: 'Noto Serif'}}>{vodText}</Text>
                </View>
            </View>
            <View>
                <Ionicons name="bookmark" color={theme.colors.onBackground} size={24}/>
            </View>
        </View>
    )
}
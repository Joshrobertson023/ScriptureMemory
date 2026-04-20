import { View, Text, TouchableOpacity } from "react-native"
import useAppTheme from "../../theme"
import React from "react";
import Ionicons from '@expo/vector-icons/Ionicons';
import useGlobalStyles from "../../styles/gobalStyles";
import { useVod } from "../../hooks/useVod";
import { useAppStore } from "../../stores/appState.store";

export const VerseOfDayHomeCard = () => {
    const theme = useAppTheme();
    const style = useGlobalStyles();
    const SAVED_MEMORIZED_VIEW_THRESHOLD = 10;

    const {vod} = useAppStore();

    if (vod.reference !== '') {
        return (
            <View style={{width: '100%', backgroundColor: theme.colors.primary, borderRadius: 10, 
                flexDirection: 'row', justifyContent: 'space-between', padding: 20}}>
                <View style={{width: '85%', justifyContent: 'space-between'}}>
                    <View>
                        <Text style={style.p4}>Verse of the Day</Text>
                        <Text style={{...style.h1, fontFamily: 'Noto Serif', fontWeight: '600'}}>{vod?.reference}</Text>
                    </View>
                    <View style={{marginTop: 50}}>
                    {vod?.verses.map((v) => (
                        <Text key={v.id} style={{...style.p2, fontFamily: 'Noto Serif', fontWeight: '400', lineHeight: 25}}>{v.reference.verses}: {v.text}</Text>
                    ))}
                    </View>
                </View>
                <View style={{display: "flex", alignItems: 'center'}}>
                    <TouchableOpacity style={{display: 'flex', alignItems: 'center', padding: 10, margin: -10}}>
                        <Ionicons name="bookmark" color={theme.colors.onBackground} size={24}/>
                        {vod?.mostSaved || 0 > SAVED_MEMORIZED_VIEW_THRESHOLD ? (
                            <Text style={style.iconText}>{vod?.mostSaved}</Text>
                        ) : (
                            <View style={{height: 15}}/>
                        )}
                    </TouchableOpacity>
                    <TouchableOpacity style={{display: 'flex', alignItems: 'center', padding: 10, margin: -10, marginTop: 5}}>
                        <Ionicons name="extension-puzzle" color={theme.colors.onBackground} size={24}></Ionicons>
                        {vod?.mostMemorized || 0 > SAVED_MEMORIZED_VIEW_THRESHOLD ? (
                            <Text style={style.iconText}>{vod?.mostMemorized}</Text>
                        ) : (
                            <View style={{height: 15}}/>
                        )}
                    </TouchableOpacity>
                </View>
            </View>
        )
    }
}
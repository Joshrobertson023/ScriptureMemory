import { StyleSheet } from 'react-native';
import useAppTheme from '../theme';
import { useMemo } from 'react';

export default function useGlobalStyles() {  
  const theme = useAppTheme();
  return useMemo(() => StyleSheet.create({
    screen: {
      flex: 1,
      backgroundColor: theme.colors.background,
      padding: 15,
      alignItems: 'center'
    },

    centerContainer: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
    },
    topContainer: {
      display: 'flex',
      justifyContent: 'flex-start',
      alignItems: 'center',
    },
    leftContainer: {
      display: 'flex',
      alignItems: 'center',
    },
    rowContainer: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      flexDirection: 'row'
    },

    bottomSheetContainer: {
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      padding: 35,
      gap: 5
    },

    h1: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 28,
      fontFamily: 'Inter',
      fontWeight: 800
    },

    p1: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 20,
      fontFamily: 'Inter',
    },
    p2: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 18,
      fontFamily: 'Inter',
    },
    p3: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 15,
      fontFamily: 'Inter',
      lineHeight: 20
    },
    p4: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 12,
      fontFamily: 'Inter'
    },

    icon: {
      width: 30,
      height: 30,
      backgroundColor: theme.colors.onBackground
    },

    iconText: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 15,
      fontWeight: 600,
      fontFamily: 'Inter'
    },

    elevationButton: {
      width: '100%',
      borderRadius: 5,
      backgroundColor: theme.colors.elevation,
      display: 'flex',
      paddingVertical: 12,
      paddingHorizontal: 25,
      justifyContent: 'center',
      alignItems: 'center'
    },
    elevationButtomSquare: {
      width: '19%',
      borderRadius: 5,
      backgroundColor: theme.colors.elevation,
      display: 'flex',
      paddingVertical: 12,
      paddingHorizontal: 0,
      justifyContent: 'center',
      alignItems: 'center',
      gap: 2
    },

    outlineButtonSkinny: {
      borderColor: theme.colors.onBackground,
      borderWidth: 1,
      borderRadius: 20,
      height: 25,
      width: '100%',
      
      justifyContent: 'center',
      alignItems: 'center',
    },
    outlineButtonSkinnyText: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 12,
      fontFamily: 'Inter'
    },

    linkButtonText: {
      textDecorationLine: 'underline',
    },
    
    input: {
      flex: 1,
      backgroundColor: theme.colors.elevation,
      color: theme.colors.onBackground,
      borderRadius: 10,
      marginBottom: 12,
      height: 50,
      borderWidth: 0.2,
      paddingLeft: 10,
    },
    
    search: {
      backgroundColor: theme.colors.elevation,
      color: theme.colors.onBackground,
      borderRadius: 40,
      marginBottom: 12,
      height: 50,
      borderWidth: 0.2,
      paddingLeft: 10,
    },

    verseReference: {
      fontFamily: 'Noto Serif',
      fontSize: 20,
      fontWeight: 600,
      color: theme.colors.onBackgroundSuperSoft
    },
    verseText: {
      fontFamily: 'Noto Serif',
      fontSize: 19,
      fontWeight: 400,
      color: theme.colors.onBackgroundSuperSoft,
      lineHeight: 29
    },
    
    /**
     * Collection cards
     */
    collectionCardsContainer: {
      display: 'flex',
      gap: 20,
      width: '100%',
      marginTop: 10
    },
    collectionCard: {
      backgroundColor: theme.colors.elevation,
      width: '100%',
      borderRadius: 10,
      height: 75,
      paddingHorizontal: 17,
      paddingVertical: 12,
      flexDirection: 'row',
      justifyContent: 'space-between'
    },
    collectionCardTitle: {
      color: theme.colors.onBackgroundSoft,
      fontSize: 18,
      fontWeight: 700,
      fontFamily: 'Inter',
      marginTop: -2
    }
  }), [theme]);

}
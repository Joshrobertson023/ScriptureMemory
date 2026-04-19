import 'react-native-gesture-handler'; // MUST be at the very top

import Ionicons from '@expo/vector-icons/Ionicons';

import * as React from 'react';
import { useEffect, useState } from 'react';
import { NavigationContainer, useNavigation } from '@react-navigation/native';
import { createNativeStackNavigator, NativeStackNavigationProp } from '@react-navigation/native-stack';

import * as SplashScreen from 'expo-splash-screen';
import * as SystemUI from 'expo-system-ui';
import { TouchableOpacity, View } from 'react-native';
import { GestureHandlerRootView } from 'react-native-gesture-handler';
import { ActivityIndicator } from 'react-native';

import useAppTheme from './theme';

import {
  useQuery,
  useMutation,
  useQueryClient,
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'
import TabsNavigator from './screens/(tabs)/TabsNavigator';
import { useUserAuthStore } from './stores/userAuth.store';
import { createUser, loginUserWithToken } from './api/user.api';
import useStyles from './styles/gobalStyles';
import { HomeScreen } from './screens/(tabs)/home.screen';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useCustomFonts } from './styles/fonts';
import { Session, useUserStore } from './stores/user.store';
import * as Device from 'expo-device';
import { CreateCollectionScreen } from './screens/collections/createNew.screen';
import PassageBottomSheet from './components/bottom-sheets/passageBottomSheet';
import { useBottomSheetsStore } from './stores/bottomSheets.store';
import { TrueSheet } from '@lodev09/react-native-true-sheet';
import { useSearchStore } from './stores/search.store';

SplashScreen.preventAutoHideAsync().catch(() => {});

const Stack = createNativeStackNavigator();

// ─── Root component ───────────────────────────────────────────────────────────
export default function AppShell() {
  const theme = useAppTheme();
  const styles = useStyles();
  const fontsLoaded = useCustomFonts();
  const authStore = useUserAuthStore();
  const userStore = useUserStore();

  const [appIsReady, setAppIsReady] = useState(false);

  const passageSheet = React.useRef<TrueSheet>(null);
  const {passageSheetOpen} = useBottomSheetsStore();
  
  // ─── Startup ──────────────────────────────────────────────────────────────────
  // On startup try to login user with auth token, retry if could not, then navigate
  async function runStartup() {
    try {
      const session: Session = {
        deviceName: Device.deviceName || '',
        model: Device.modelId,
      }
      if (authStore.refreshToken) { // If refresh token, automatically login (only users who created an account have a refresh token)
        console.log('logging in with token')
        await loginUserWithToken(session);
      } else if (authStore.session.deviceId) {
        // Get new jwt if internet connection
        console.log('requesting new jwt')
      } else if (!authStore.session.deviceId) {
        // New user
        console.log('creating new user')
        await createUser(session);
      }
    } catch (Error) {
      console.error(Error);
    }

    setAppIsReady(true);
  }
  
  useEffect(() => {
      runStartup();
    }, []); // ← IMPORTANT
    
    // ── Android system background ──────────────────────────────────────────────────
    useEffect(() => {
      SystemUI.setBackgroundColorAsync(theme.colors.background).catch((e) =>
        console.warn('Failed to set system UI background:', e),
      );
    }, [theme.colors.background]);
  
  
    // ── Hide splash screen once ready ────────────────────────────────────────
    useEffect(() => {
      if (appIsReady) 
        if (fontsLoaded) 
            SplashScreen.hideAsync().catch(() => {});
    }, [appIsReady, fontsLoaded]);

    // set passage bottom sheet ref
    useEffect(() => {
      if (passageSheetOpen) {
        passageSheet.current?.present();
      } else {
        passageSheet.current?.dismiss();
      }
    }, [passageSheetOpen])

  if (!appIsReady || !fontsLoaded) {
    return <ActivityIndicator />;
  } 


    return (
        
      <GestureHandlerRootView style={{ flex: 1, backgroundColor: theme.colors.background }}>
            <NavigationContainer theme={theme}>
              <Stack.Navigator
                screenOptions={{ contentStyle: { backgroundColor: theme.colors.background } }}
              >
                
                <Stack.Screen 
                  name="(tabs)" 
                  component={TabsNavigator} 
                  options={{ headerShown: false, animation: 'none' }} />
                <Stack.Screen
                  name="createCollection"
                  component={CreateCollectionScreen}
                  options={{
                    headerShown: true,
                    headerTitle: 'New Collection',
                    animation: 'default',
                    headerStyle: {
                      backgroundColor: theme.colors.background2,
                    },
                    headerTitleStyle: {
                      color: theme.colors.onBackground,
                      fontSize: 20
                    },
                    headerTintColor: theme.colors.onBackground,
                  }}
                />
              </Stack.Navigator>
            </NavigationContainer>
          <PassageBottomSheet ref={passageSheet}/>
      </GestureHandlerRootView>
    )
}
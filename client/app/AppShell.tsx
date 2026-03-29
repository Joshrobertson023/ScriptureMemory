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
import { loginUserWithToken } from './api/user.api';
import useStyles from './styles/gobalStyles';
import { HomeScreen } from './screens/(tabs)/home.screen';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useCustomFonts } from './styles/fonts';

SplashScreen.preventAutoHideAsync().catch(() => {});

const Stack = createNativeStackNavigator();

// ─── Root component ───────────────────────────────────────────────────────────
export default function AppShell() {
  const theme = useAppTheme();
  const styles = useStyles();
  const fontsLoaded = useCustomFonts();

  const [appIsReady, setAppIsReady] = useState(false);
  
  // ─── Startup ──────────────────────────────────────────────────────────────────
  const loadToken = useUserAuthStore(s => s.loadToken);
  const token = useUserAuthStore(s => s.authToken);
  const setToken = useUserAuthStore(s => s.setToken);
  const isAuthenticated = useUserAuthStore(s => s.isAuthenticated);

  // Login mutation
    const loginMutation = useMutation({
        mutationFn: (data: { authToken: string; }) => loginUserWithToken(token),
        onSuccess: async (data) => {
            if (data.authToken) {
              await setToken(data.authToken);
            }
            else {
              console.log("Error logging in with stored token.");
              return;
            }
        }   
    })

  // On startup try to login user with auth token, retry if could not, then navigate
  async function runStartup() {
    // await loadToken();

    // if (token) {
    //   try {
    //     await loginMutation.mutateAsync({ authToken: token });
    //   } catch {}
    // }

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
                
                                {/* Settings & info screens */}
                {/* <Stack.Screen name="privacy"       component={PrivacyScreen}       options={{ title: 'Privacy Policy',              ...sharedHeaderStyle }} />
                <Stack.Screen name="terms"         component={TermsScreen}         options={{ title: 'Terms of Service',            ...sharedHeaderStyle }} />
                <Stack.Screen name="activity"      component={ActivityScreen}      options={{ title: 'Activity Tracking & Sharing', ...sharedHeaderStyle }} />
                <Stack.Screen name="about"         component={AboutScreen}         options={{ title: 'About',                       ...sharedHeaderStyle }} />
                <Stack.Screen name="notifications" component={NotificationsScreen} options={{ title: 'Notifications',               ...sharedHeaderStyle }} />
                <Stack.Screen name="admin"         component={AdminScreen}         options={{ title: 'Admin Panel',                 ...sharedHeaderStyle }} />
                <Stack.Screen name="settings"      component={SettingsScreen}      options={{ title: 'Settings',                    ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="practiceSession" component={PracticeSessionScreen} options={{ title: 'Practice',               ...sharedHeaderStyle }} /> */}

                {/* Profile / content screens */}
                {/* <Stack.Screen name="user"     component={UserScreen}     options={{ headerShown: false }} />
                <Stack.Screen name="book"     component={BookScreen}     options={{ headerShown: false }} />
                <Stack.Screen name="chapters" component={ChaptersScreen} options={{ headerShown: false }} /> */}

                {/* Collections screens */}
                {/* <Stack.Screen name="collections/addnew"                component={AddNewCollectionScreen}       options={{ title: 'New Collection',      ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="collections/reorderCollections"    component={ReorderCollectionsScreen}     options={{ title: 'Reorder Collections',  ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="collections/reorderVerses"         component={ReorderVersesScreen}          options={{ title: 'Reorder Passages',     ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="collections/reorderExistingVerses" component={ReorderExistingVersesScreen}  options={{ title: 'Reorder Passages',     ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="collections/editCollection"        component={EditCollectionScreen}         options={{ title: 'Edit Collection',      ...sharedHeaderStyleNoShadow }} />
                <Stack.Screen name="collections/publishCollection"     component={PublishCollectionScreen}      options={{ title: 'Publish Collection',   ...sharedHeaderStyleNoShadow }} /> */}

                {/* <Stack.Screen
                  name="collections/[id]"
                  component={CollectionDetailScreen}
                  options={{
                    title: '',
                    ...sharedHeaderStyleNoShadow,
                    headerRight: () => (
                      <View style={{ flexDirection: 'row', gap: 15, marginRight: 10 }}>
                        <TouchableOpacity onPress={() => {}}>
                          <Ionicons
                            style={{ marginTop: 4 }}
                            name="ellipsis-vertical"
                            size={32}
                            color={theme.colors.onBackground}
                          />
                        </TouchableOpacity>
                      </View>
                    ),
                  }}
                /> */}

                {/* <Stack.Screen
                  name="explore/collection/[id]"
                  component={ExploreCollectionScreen}
                  options={{ title: '', ...sharedHeaderStyle }}
                /> */}
              </Stack.Navigator>
            </NavigationContainer>
      </GestureHandlerRootView>
    )
}
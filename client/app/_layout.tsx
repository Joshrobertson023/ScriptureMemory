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
import { ActivityIndicator, PaperProvider, Portal, Text } from 'react-native-paper';

import useAppTheme from './theme';
import AuthNavigator from './screens/(auth)/_layout';

import {
  useQuery,
  useMutation,
  useQueryClient,
  QueryClient,
  QueryClientProvider,
} from '@tanstack/react-query'
import TabsNavigator from './screens/(tabs)/_layout';
import { useUserAuthStore } from './stores/userAuth.store';
import { loginUserWithToken } from './api/user.api';
import useStyles from './styles';

SplashScreen.preventAutoHideAsync().catch(() => {});

// Tells Typescript what routes are available
export type RootStackParamList = {
  '(tabs)': undefined;
  '(auth)': undefined;
};
const Stack = createNativeStackNavigator<RootStackParamList>();

const queryClient = new QueryClient();


// ─── Root component ───────────────────────────────────────────────────────────
export default function RootLayout() {
  const theme = useAppTheme();
  const styles = useStyles();

  const [appIsReady, setAppIsReady] = useState(false);

  const sharedHeaderStyle = {
    headerStyle: { backgroundColor: theme.colors.background },
    headerTitleStyle: { color: theme.colors.onBackground },
    headerTintColor: theme.colors.onBackground,
  } as const;

  const sharedHeaderStyleNoShadow = {
    ...sharedHeaderStyle,
    headerShadowVisible: false,
  } as const;

  type NavigationProp = NativeStackNavigationProp<RootStackParamList>;
  const navigation = useNavigation<NavigationProp>();
  
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
            navigation.navigate("(auth)");
            return;
          }
          navigation.navigate('(tabs)');
      }   
  })

  async function runStartup() {
    await loadToken();
    let retryCount = 3;
    if (token !== '') {
      for (let i = 0; i < retryCount; i++, retryCount++) {
        await loginMutation.mutateAsync({authToken: token})
        if (isAuthenticated) break;
      }
      if (isAuthenticated)
        navigation.navigate("(tabs)");
    } else {
      navigation.navigate("(auth)");
      console.log("No stored auth token found, navigating to (auth)");
    }
  }

  useEffect(() => {
    runStartup();
  })

  // ── Android system background ──────────────────────────────────────────────────
  useEffect(() => {
    SystemUI.setBackgroundColorAsync(theme.colors.background).catch((e) =>
      console.warn('Failed to set system UI background:', e),
    );
  }, [theme.colors.background]);


  // ── Hide splash screen once ready ────────────────────────────────────────
  useEffect(() => {
    if (appIsReady) SplashScreen.hideAsync().catch(() => {});
  }, [appIsReady]);

  if (!appIsReady) 
    return (
      <Text style={styles.text}>An error has occurred. Please restart the app.</Text>
    );
  
  // ── Main navigator ────────────────────────────────────────────────────────
  return (
    <GestureHandlerRootView style={{ flex: 1, backgroundColor: theme.colors.background }}>
      <PaperProvider theme={theme}>
        <Portal.Host>
          <NavigationContainer>
            <QueryClientProvider client={queryClient}>
              <Stack.Navigator
                screenOptions={{ contentStyle: { backgroundColor: theme.colors.background } }}
              >
                {/* Tab / auth roots */}
                <Stack.Screen name="(tabs)"  component={TabsNavigator}  options={{ headerShown: false, animation: 'none' }} />
                <Stack.Screen name="(auth)"  component={AuthNavigator}  options={{ headerShown: false }} />

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
            </QueryClientProvider>
          </NavigationContainer>
        </Portal.Host>
      </PaperProvider>
    </GestureHandlerRootView>
  );
}

import * as SystemUI from 'expo-system-ui';
import React from 'react';
import useAppTheme from '../../theme';

import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

import CreateAccountScreen from "./createAccount.screen";
import { QueryClientProvider } from '@tanstack/react-query';
import LoginScreen from './login.screen';


const Stack = createNativeStackNavigator();

export default function AuthNavigator() {

  const theme = useAppTheme();
  SystemUI.setBackgroundColorAsync(theme.colors.background);

    return ( 
      <Stack.Navigator initialRouteName="login">
        <Stack.Screen 
        name="login" 
        component={LoginScreen} 
        options={{ 
          headerShown: false, 
          headerStyle: {
            backgroundColor: theme.colors.background,
          },
          headerShadowVisible: false
        }} />
        <Stack.Screen 
        name="createAccount" 
        component={CreateAccountScreen} 
        options={{ 
          headerShown: false, 
          headerStyle: {
            backgroundColor: theme.colors.background,
          },
          headerShadowVisible: false
        }} />
      </Stack.Navigator>
  )
}
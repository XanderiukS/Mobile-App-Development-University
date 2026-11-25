import {NavigationContainer} from "@react-navigation/native";
import {createNativeStackNavigator} from "@react-navigation/native-stack";
import HomeScreen from "./pantallas/HomeScreen";
import ClimaScreen from "./pantallas/ClimaScreen";

const Stack = createNativeStackNavigator();

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{headerShown: false}}>
        <Stack.Screen name="Home" component={HomeScreen} />
        <Stack.Screen name="Clima" component={ClimaScreen} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}
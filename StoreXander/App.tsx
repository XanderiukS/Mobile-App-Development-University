import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  Pressable,
  StyleSheet,
  ActivityIndicator,
  FlatList,
  Keyboard,
  StatusBar,
} from 'react-native';
import { SafeAreaProvider, SafeAreaView } from 'react-native-safe-area-context';
import { buscarProductos, Producto } from './src/services/productService';
import ProductItem from './src/components/ProductItem';

export default function App() {
  const [texto, setTexto] = useState('');
  const [productos, setProductos] = useState<Producto[]>([]);
  const [cargando, setCargando] = useState(false);
  const [error, setError] = useState(''); // error de conexión/servicio
  const [mensaje, setMensaje] = useState(''); // mensaje de validación
  const [busquedaRealizada, setBusquedaRealizada] = useState(false);
  const [ultimaBusqueda, setUltimaBusqueda] = useState(''); // para "Reintentar"

  // REALIZA LA CONSULTA AL SERVICIO CON EL TEXTO RECIBIDO
  const consultar = async (consulta: string) => {
    setCargando(true);
    setError('');
    setMensaje('');
    setProductos([]);
    setBusquedaRealizada(false);
    setUltimaBusqueda(consulta);

    //LLAMADA AL SERVICIO DE PRODUCTOS
    try {
      const resultado = await buscarProductos(consulta);
      setProductos(resultado); //GUARDAR LOS PRODUCTOS EN EL ESTADO
      setBusquedaRealizada(true);
    } catch (e) {
      console.error(e);
      setError(
        'No se pudo conectar con el servicio. Verifique su conexión a internet.',
      );
    }
    setCargando(false);
  };

  // BOTON BUSCAR: valida el texto antes de consultar
  const Buscar = () => {
    Keyboard.dismiss();
    if (!texto.trim()) {
      setMensaje('No se encontraron productos, Por Favor ingresa uno');
      setError('');
      setProductos([]);
      setBusquedaRealizada(false);
      return;
    }
    consultar(texto);
  };

  // BOTON REINTENTAR: repite la última búsqueda
  const Reintentar = () => {
    if (ultimaBusqueda) {
      consultar(ultimaBusqueda);
    }
  };

  // BOTON LIMPIAR: borra texto, resultados y mensajes
  const Limpiar = () => {
    Keyboard.dismiss();
    setTexto('');
    setProductos([]);
    setError('');
    setMensaje('');
    setBusquedaRealizada(false);
    setUltimaBusqueda('');
  };

  return (
    <SafeAreaProvider>
      <StatusBar barStyle="dark-content" />
      <SafeAreaView style={styles.container}>
        {/* FORMULARIO DE BUSQUEDA */}
        <View style={styles.form}>
          <Text style={styles.title}>StoreXander</Text>
          <Text style={styles.subtitle}>Catálogo de Productos</Text>

          <TextInput
            style={styles.input}
            placeholder="Ingrese el producto"
            placeholderTextColor="#888"
            value={texto}
            onChangeText={setTexto}
            onSubmitEditing={Buscar}
            returnKeyType="search"
            autoCapitalize="none"
          />

          <View style={styles.botones}>
            <Pressable
              style={({ pressed }) => [
                styles.button,
                styles.buttonBuscar,
                pressed && styles.buttonPresionado,
              ]}
              onPress={Buscar}
              disabled={cargando}
            >
              <Text style={styles.buttonText}>Buscar</Text>
            </Pressable>
            <Pressable
              style={({ pressed }) => [
                styles.button,
                styles.buttonLimpiar,
                pressed && styles.buttonPresionado,
              ]}
              onPress={Limpiar}
            >
              {({ pressed }) => (
                <Text
                  style={pressed ? styles.buttonText : styles.buttonTextLimpiar}
                >
                  Limpiar
                </Text>
              )}
            </Pressable>
          </View>
        </View>

        {cargando && (
          <ActivityIndicator
            size="large"
            color="#333"
            style={styles.loader}
          />
        )}

        {/* MENSAJE DE VALIDACION */}
        {mensaje ? <Text style={styles.validacion}>{mensaje}</Text> : null}

        {/* MENSAJE DE ERROR*/}
        {error ? (
          <View style={styles.errorBox}>
            <Text style={styles.error}>{error}</Text>
            <TouchableOpacity onPress={Reintentar}>
              <Text style={styles.reintentar}>Reintentar</Text>
            </TouchableOpacity>
          </View>
        ) : null}

        {/* SIN RESULTADOS */}
        {busquedaRealizada && !cargando && !error && productos.length === 0 ? (
          <Text style={styles.vacio}>No se encontraron productos</Text>
        ) : null}

        {/* LISTA */}
        <FlatList
          data={productos}
          keyExtractor={item => item.id.toString()}
          renderItem={({ item }) => <ProductItem producto={item} />}
          contentContainerStyle={styles.lista}
          keyboardShouldPersistTaps="handled"
          ListHeaderComponent={
            productos.length > 0 ? (
              <Text style={styles.contador}>
                {productos.length} producto(s) encontrado(s)
              </Text>
            ) : undefined
          }
        />
      </SafeAreaView>
    </SafeAreaProvider>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#fff',
  },
  form: {
    padding: 20,
    paddingBottom: 10,
  },
  title: {
    color: '#222',
    fontSize: 26,
    fontWeight: 'bold',
    textAlign: 'center',
  },
  subtitle: {
    color: '#777',
    fontSize: 15,
    textAlign: 'center',
    marginBottom: 16,
  },
  input: {
    padding: 12,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ccc',
    color: '#000',
    fontSize: 16,
    marginBottom: 10,
  },
  botones: {
    flexDirection: 'row',
    gap: 10,
  },
  button: {
    flex: 1,
    padding: 12,
    borderRadius: 8,
    alignItems: 'center',
  },
  buttonBuscar: {
    backgroundColor: '#222',
  },
  buttonLimpiar: {
    backgroundColor: '#eee',
  },
  buttonPresionado: {
    backgroundColor: '#d32f2f',
  },
  buttonText: {
    color: '#fff',
    fontWeight: '600',
    fontSize: 16,
  },
  buttonTextLimpiar: {
    color: '#333',
    fontWeight: '600',
    fontSize: 16,
  },
  loader: {
    marginVertical: 20,
  },
  validacion: {
    color: '#c62828',
    fontSize: 14,
    marginHorizontal: 20,
  },
  errorBox: {
    marginHorizontal: 20,
    alignItems: 'center',
  },
  error: {
    color: '#c62828',
    textAlign: 'center',
    fontSize: 14,
    marginBottom: 8,
  },
  reintentar: {
    color: '#222',
    fontWeight: '600',
    fontSize: 15,
    textDecorationLine: 'underline',
  },
  vacio: {
    color: '#777',
    textAlign: 'center',
    fontSize: 15,
    marginTop: 20,
  },
  lista: {
    paddingHorizontal: 20,
    paddingBottom: 20,
  },
  contador: {
    color: '#777',
    fontSize: 13,
    marginBottom: 4,
  },
});

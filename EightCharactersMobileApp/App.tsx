import React, { useState } from 'react';
import {
  Text,
  TextInput,
  Button,
  Alert,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';

const FormularioUsuario = () => {
  const [nombre, setNombre] = useState('');
  const [apellido, setApellido] = useState('');
  const [edad, setEdad] = useState('');
  const [telefono, setTelefono] = useState('');
  const [correo, setCorreo] = useState('');
  const [direccion, setDireccion] = useState('');
  const [dpi, setDpi] = useState('');
  const [nacionalidad, setNacionalidad] = useState('');

  const handleSubmit = () => {
    // Verificamos que todos los campos esten llenos al hacer Submit
    if (
      !nombre ||
      !apellido ||
      !edad ||
      !telefono ||
      !correo ||
      !direccion ||
      !dpi ||
      !nacionalidad
    ) {
      Alert.alert('Error', 'Por favor, ingrese los campos completos');
    } else {
      Alert.alert(
        'Datos del Formulario',
        `Nombre: ${nombre}\nApellido: ${apellido}\nEdad: ${edad}\nTeléfono: ${telefono}\nCorreo: ${correo}\nDirección: ${direccion}\nDPI: ${dpi}\nNacionalidad: ${nacionalidad}`,
      );
    }
  };

  return (
    <KeyboardAvoidingView
      style={style.flex}
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}>
      <ScrollView
        contentContainerStyle={style.container}
        keyboardShouldPersistTaps="handled">
        <Text style={style.label}> Nombre:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu nombre"
          value={nombre}
          onChangeText={setNombre}
        />

        <Text style={style.label}> Apellido:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu apellido"
          value={apellido}
          onChangeText={setApellido}
        />

        <Text style={style.label}> Edad:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu edad"
          keyboardType="numeric"
          value={edad}
          onChangeText={setEdad}
        />

        <Text style={style.label}> Teléfono:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu teléfono"
          keyboardType="phone-pad"
          value={telefono}
          onChangeText={setTelefono}
        />

        <Text style={style.label}> Correo:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu correo"
          keyboardType="email-address"
          autoCapitalize="none"
          value={correo}
          onChangeText={setCorreo}
        />

        <Text style={style.label}> Dirección:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu dirección"
          value={direccion}
          onChangeText={setDireccion}
        />

        <Text style={style.label}> DPI:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu DPI"
          keyboardType="numeric"
          value={dpi}
          onChangeText={setDpi}
        />

        <Text style={style.label}> Nacionalidad:</Text>
        <TextInput
          style={style.input}
          placeholder="Escribe tu nacionalidad"
          value={nacionalidad}
          onChangeText={setNacionalidad}
        />

        <Button title="Enviar" onPress={handleSubmit} />
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

const style = StyleSheet.create({
  flex: {
    flex: 1,
  },
  container: {
    padding: 20,
    paddingTop: 50,
  },
  label: {
    fontWeight: 'bold',
    marginBottom: 5,
  },
  input: {
    height: 40,
    borderColor: '#aaa',
    borderWidth: 1,
    marginBottom: 15,
    paddingHorizontal: 10,
    borderRadius: 5,
  },
});

export default FormularioUsuario;

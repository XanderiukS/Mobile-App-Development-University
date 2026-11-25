import React from 'react';
import { View, Text, Image, StyleSheet } from 'react-native';
import { Producto } from '../services/productService';

interface Props {
  producto: Producto;
}

// INFORMACION DE UN PRODUCTO
export default function ProductItem({ producto }: Props) {
  return (
    <View style={styles.item}>
      {/* MINIATURA */}
      <Image source={{ uri: producto.thumbnail }} style={styles.image} />

      <View style={styles.info}>
        <Text style={styles.title}>{producto.title}</Text>
        <Text style={styles.description} numberOfLines={2}>
          {producto.description}
        </Text>

        {/* PRECIO */}
        <Text style={styles.price}>${producto.price.toFixed(2)}</Text>
        <Text style={styles.detalle}>
          Categoría: {producto.category} · Existencias: {producto.stock}
        </Text>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  item: {
    flexDirection: 'row',
    paddingVertical: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  image: {
    width: 70,
    height: 70,
    borderRadius: 6,
    backgroundColor: '#f2f2f2',
    marginRight: 12,
  },
  info: {
    flex: 1,
  },
  title: {
    color: '#222',
    fontSize: 16,
    fontWeight: '600',
  },
  description: {
    color: '#666',
    fontSize: 13,
    marginVertical: 2,
  },
  price: {
    color: '#222',
    fontSize: 15,
    fontWeight: 'bold',
  },
  detalle: {
    color: '#888',
    fontSize: 12,
    marginTop: 2,
  },
});

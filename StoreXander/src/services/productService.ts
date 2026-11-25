// SERVICIO DE PRODUCTOS
const URL_BASE = 'https://dummyjson.com/products/search';

export interface Producto {
  id: number;
  title: string;
  description: string;
  price: number;
  category: string;
  stock: number;
  thumbnail: string;
}

// BUSCA PRODUCTOS
export async function buscarProductos(texto: string): Promise<Producto[]> {
  const url = `${URL_BASE}?q=${encodeURIComponent(texto.trim())}`;

  const response = await fetch(url);

  if (!response.ok) {
    throw new Error(`Error del servidor: ${response.status}`);
  }

  const data = await response.json(); // LEER
  return data.products ?? [];
}

import React, { useEffect, useState } from 'react'

const API = (import.meta.env.VITE_API_BASE || 'api')

export default function App() {
  const [items, setItems] = useState([])
  const [name, setName] = useState('')
  const [price, setPrice] = useState('')

  const load = async () => {
    const res = await fetch(`${API}/api/products`)
    const data = await res.json()
    setItems(data)
  }

  const add = async (e) => {
    e.preventDefault()
    await fetch(`${API}/api/products`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, price: parseFloat(price), inStock: true })
    })
    setName(''); setPrice('')
    load()
  }

  useEffect(() => { load() }, [])

  return (
    <div style={{ fontFamily: 'sans-serif', padding: 16 }}>
      <h1>DevOps 3‑Tier Demo</h1>
      <form onSubmit={add} style={{ marginBottom: 16 }}>
        <input placeholder="Name" value={name} onChange={e => setName(e.target.value)} />
        <input placeholder="Price" value={price} onChange={e => setPrice(e.target.value)} type="number" step="0.01" />
        <button>Add</button>
      </form>
      <ul>
        {items.map(p => (
          <li key={p.id}>{p.name} — ₹{p.price}</li>
        ))}
      </ul>
    </div>
  )
}

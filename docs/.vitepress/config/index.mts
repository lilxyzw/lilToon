import { defineConfig } from 'vitepress'
import { shared } from './shared'
import { ja_JP } from './ja_JP'

export default defineConfig({
  ...shared,
  locales: {
    ja_JP: { label: '日本語', ...ja_JP },
  }
})

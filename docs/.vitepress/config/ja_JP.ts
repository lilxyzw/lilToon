import { defineConfig } from 'vitepress'

const langName = '/ja_JP';

export const ja_JP = defineConfig({
  lang: 'ja_JP',
  description: "アバターを用いたサービス向けに開発した多機能トゥーンシェーダーです。",
  themeConfig: {
    logo: '/images/logo.svg',
    nav: [
      { text: 'ホーム', link: langName + '/' },
      { text: 'ドキュメント', link: langName + '/docs/', activeMatch: '/docs/' }
    ],
    sidebar: [
      { text: 'はじめに', link: langName + '/first' },
      {
        text: '基本設定',
        collapsed: false,
        items: [
          { text: '基本設定', link: langName + '/base/base' },
          { text: 'FakeShadow設定', link: langName + '/base/fakeshadow' },
          { text: 'ライティング設定', link: langName + '/base/lighting' },
          { text: 'UV設定', link: langName + '/base/uv' },
          { text: 'VRChat', link: langName + '/base/vrchat' },
        ]
      },
      {
        text: '色設定',
        collapsed: false,
        items: [
          { text: 'メインカラー / 透過', link: langName + '/color/maincolor' },
          { text: 'メインカラー2nd・3rd', link: langName + '/color/maincolor_layer' },
          { text: 'アルファマスク', link: langName + '/color/alphamask' },
          { text: '影設定', link: langName + '/color/shadow' },
          { text: 'RimShade', link: langName + '/color/rimshade' },
          { text: '発光設定', link: langName + '/color/emission' },
        ]
      },
      {
        text: 'ノーマルマップ・光沢設定',
        collapsed: false,
        items: [
          { text: 'ノーマルマップ設定', link: langName + '/reflections/normal' },
          { text: '異方性反射', link: langName + '/reflections/anisotropy' },
          { text: '逆光ライト', link: langName + '/reflections/backlight' },
          { text: '光沢設定', link: langName + '/reflections/reflection' },
          { text: 'マットキャップ設定', link: langName + '/reflections/matcap' },
          { text: 'リムライト設定', link: langName + '/reflections/rimlight' },
          { text: 'ラメ設定', link: langName + '/reflections/glitter' },
          { text: '宝石設定', link: langName + '/reflections/gem' },
        ]
      },
      {
        text: '拡張設定',
        collapsed: false,
        items: [
          { text: '輪郭線設定', link: langName + '/advanced/outline' },
          { text: '視差マップ', link: langName + '/advanced/parallax' },
          { text: '距離フェード', link: langName + '/advanced/distancefade' },
          { text: 'AudioLink', link: langName + '/advanced/audiolink' },
          { text: 'Dissolve', link: langName + '/advanced/dissolve' },
          { text: 'ステンシル設定', link: langName + '/advanced/stencil' },
          { text: 'レンダリング設定', link: langName + '/advanced/rendering' },
          { text: 'テッセレーション', link: langName + '/advanced/tessellation' },
          { text: '屈折設定', link: langName + '/advanced/refraction' },
          { text: 'ファー設定', link: langName + '/advanced/fur' },
        ]
      },
      {
        text: 'その他',
        collapsed: false,
        items: [
          { text: '最適化', link: langName + '/other/optimization' },
          { text: 'シェーダー設定', link: langName + '/other/settings' },
          { text: 'メニューの追加項目', link: langName + '/other/menuitem' },
          { text: 'テクスチャのインポート設定について', link: langName + '/other/textures' },
          { text: 'Q&A', link: langName + '/other/qa' },
          { text: '変更履歴', link: langName + '/other/changelog' },
        ]
      },
      {
        text: '開発者向けドキュメント',
        collapsed: false,
        items: [
          { text: 'ファイル構成', link: langName + '/dev/files' },
          { text: 'シェーダーの構造', link: langName + '/dev/shader_structure' },
          { text: 'カスタムシェーダーの作り方', link: langName + '/dev/custom_shader' },
          { text: 'ユーティリティ', link: langName + '/dev/utilities' },
          { text: 'カスタムシェーダーの仕様', link: langName + '/dev/custom_shader_format' },
        ]
      },
    ],
    search: {
      provider: 'local',
      options: {
        locales: {
          ja_JP: {
            translations: {
              button: {
                buttonText: '検索',
                buttonAriaLabel: '検索'
              },
              modal: {
                displayDetails: '詳細リストを表示',
                resetButtonTitle: '検索条件を削除',
                backButtonTitle: '検索を閉じる',
                noResultsText: '見つかりませんでした。',
                footer: {
                  selectText: '選択',
                  selectKeyAriaLabel: 'エンター',
                  navigateText: '切り替え',
                  navigateUpKeyAriaLabel: '上矢印',
                  navigateDownKeyAriaLabel: '下矢印',
                  closeText: '閉じる',
                  closeKeyAriaLabel: 'エスケープ'
                }
              }
            }
          }
        }
      }
    },
    lastUpdated: {
      text: '最終更新',
      formatOptions: {
        dateStyle: 'full',
        timeStyle: 'medium'
      }
    }
  }
})

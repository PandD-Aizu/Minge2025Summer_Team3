# CreditScreen

映画のエンドロール風にテキストが下から上にスクロールするクレジット画面システム。

## ファイル構成

```
CreditScreen/
├── Scripts/         # MVPパターンで実装されたスクリプト群
├── Prefabs/         # 使用可能なUIプレハブ
├── Scenes/          # テスト用シーン
└── ScriptableObjects/ # クレジットデータ
```

## 基本的な使い方

### 1. 配置
- CreditScreenCanvas.prefabをシーンに配置
- ScrollViewオブジェクトに CreditScreenView と CreditScreenPresenter をアタッチ

### 2. データ作成
```
右クリック → Create → CreditScreen → CreditData
```

### 3. 設定
CreditScreenPresenterのCredit Dataフィールドに作成したアセットを設定。

## CreditDataの設定項目

### 基本情報
- Game Title: ゲームタイトル
- Credit Sections: クレジットのセクション一覧

### セクション構成
各セクションには以下を設定：
- Section Title: セクション名
- Credits: そのセクションの人名リスト

### スクロール設定
- Scroll Speed: スクロール速度（pixel/秒）
- Delay Before Start: 開始前の待機時間
- Delay After End: 終了後の待機時間

## 機能

- 自動スクロール
- Escキーでのスキップ
- シーン遷移
- ループ再生
- 自動サイズ調整

## UI設定

### CreditScreenView
- UI Components: 各UI要素の参照設定
- Visual Settings: 色や行間の調整

### CreditScreenPresenter  
- Scene Transition: 次のシーン名、ループ設定
- Skip設定: スキップキーの変更

## 注意事項

- Textコンポーネントは日本語対応フォント使用推奨
- Content Size Fitterは自動で無効化される
- ScrollRectのVerticalをオンにする
- Rich Textをオンにする（太字タグ使用のため）
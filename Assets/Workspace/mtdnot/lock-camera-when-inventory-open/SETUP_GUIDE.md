# カメラロック機能 セットアップガイド

## 問題の解決方法

インベントリを開いてもマウス操作で視点が動いてしまう問題を解決します。

## 実装ファイル

**ImprovedCameraLockManager.cs** - CinemachinePanTiltとCinemachineInputAxisControllerを制御

## セットアップ手順

### 1. インベントリUIオブジェクトに追加

1. Hierarchyでインベントリ画面のGameObjectを選択
2. `ImprovedCameraLockManager.cs`をAdd Component
3. **Auto Find Components**にチェックを入れる（自動検索）

### 2. 手動設定（自動検索で見つからない場合）

Inspectorで以下を設定：

#### Camera Components to Control
- **Pan Tilt Components**: 
  - 青でハイライトされているオブジェクトの`CinemachinePanTilt`コンポーネントをドラッグ
  - Virtual CameraのPanTiltコンポーネント
  
- **Input Axis Controllers**:
  - Virtual Cameraの`CinemachineInputAxisController`をドラッグ

#### Optional: Additional Controls
- **Third Person Controller**: Player GameObject
- **Starter Assets Inputs**: Player Input
- **Player Movement Controllers**: 移動制御GameObjectのリスト

### 3. 動作確認

1. **Debug Mode**をオンにする
2. Playモードで実行
3. インベントリを開く（Tab キー）
4. Consoleログを確認：
   ```
   [CameraLockManager] Disabled PanTilt: [オブジェクト名]
   [CameraLockManager] === CAMERA LOCKED ===
   ```
5. マウスを動かしてもカメラが動かないことを確認

## トラブルシューティング

### カメラが固定されない場合

1. **Consoleに警告が出ている場合**
   ```
   No CinemachinePanTilt components assigned!
   ```
   → 青でハイライトされているオブジェクトのPanTiltコンポーネントを手動で設定

2. **特定のコンポーネントだけ動く場合**
   - 全てのVirtual CameraのPanTilt/InputAxisControllerを追加
   - 複数ある場合はリストに全て追加

3. **OnEnable/OnDisableが呼ばれない場合**
   - インベントリUIのSetActive(true/false)を確認
   - 手動でLockCamera()/UnlockCamera()を呼ぶ

## 使用例

```csharp
// 他のスクリプトから呼ぶ場合
var cameraLock = GetComponent<ImprovedCameraLockManager>();
cameraLock.LockCamera();   // カメラ固定
cameraLock.UnlockCamera(); // カメラ固定解除
cameraLock.ToggleLock();  // 切り替え
```

## 重要な注意点

- **CinemachinePanTilt**が青でハイライトされているオブジェクトにある
- このコンポーネントの.enabledを切り替えることが重要
- CinemachineInputAxisControllerも同時に制御する必要がある
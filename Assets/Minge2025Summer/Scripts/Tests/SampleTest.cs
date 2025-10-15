using System.Collections;
using Minge2025Summer.Scripts.Sample;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UniRx;

namespace Minge2025Summer.Scripts.Tests
{
    /// <summary>
    /// このクラスは、PlayMode（実際のフレーム進行あり）で動くUnityのテストです。
    /// PlayerHPModelのHP変化通知と、PlayerHPPresenterのログ出力が正しく動くかを確認します。
    /// </summary>
    public class SampleTest
    {
        [UnityTest]
        public IEnumerator PlayerHPModel_TakeDamage_ChangesHPAndNotifies()
        {
            // テスト用の空のGameObjectを作り、テスト対象のコンポーネントを追加します。
            var go = new GameObject();
            var model = go.AddComponent<PlayerHPModel>();

            // HP変更通知（OnHPChanged）を受け取るための変数。
            int notifiedHp = -1;

            // UniRxのSubscribeでイベントを購読。HPが変わるとnotifiedHpに値が入ります。
            model.OnHPChanged.Subscribe(hp => notifiedHp = hp);

            // ダメージを与えてHPを90に減らす（初期値100前提）。
            model.TakeDamage(10);

            // PlayModeのテストでは、イベントが次のフレームで処理されることがあるため1フレーム待ちます。
            yield return null;

            // 通知で受け取ったHPが90になっていることを確認。
            Assert.AreEqual(90, notifiedHp);

            // リフレクションでprivateフィールドcurrentHPの実際の値も確認。
            var field = model
                .GetType()
                .GetField("currentHP", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(field, "currentHPフィールドが見つかりません");
            Assert.AreEqual(90, field.GetValue(model));

            // テスト用オブジェクトを破棄（PlayModeではDestroyを使用）。
            Object.Destroy(go);
        }

        [UnityTest]
        public IEnumerator PlayerHPPresenter_LogsOnHPChanged()
        {
            // モデルとプレゼンターを別のGameObjectにそれぞれ追加して準備します。
            var go = new GameObject();
            var model = go.AddComponent<PlayerHPModel>();
            var presenterGo = new GameObject();
            var presenter = presenterGo.AddComponent<PlayerHPPresenter>();

            // Presenterが参照するmodelフィールドに、作成したmodelを差し込みます（依存の注入）。
            var modelField = presenter
                .GetType()
                .GetField("model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(modelField, "modelフィールドが見つかりません");
            modelField.SetValue(presenter, model);

            // MonoBehaviourのStartは生成直後のフレームではまだ呼ばれていない可能性があるため、
            // 1フレーム待ってStart内の購読処理が完了するのを待ちます。
            yield return null;

            // LogAssert.Expectは、期待するログを「出力前」に設定しておく必要があります。
            LogAssert.Expect(LogType.Log, "Player HP changed: 90");

            // HPを減らすと、Presenterがログ（Debug.Log）を出力する想定です。
            model.TakeDamage(10);

            // ログの反映を待つために1フレーム待機します。
            yield return null;

            // 片付け：生成したオブジェクトを破棄します。
            Object.Destroy(go);
            Object.Destroy(presenterGo);
        }
    }
}
# CancellationTokenをdatabase処理まで伝播する: 回答シート

教材: [10-cancellation-token-propagation.md](10-cancellation-token-propagation.md)

状態: 完了

## 問1

endpointとserviceが`CancellationToken`を受け取っていても、EF Coreのasync methodへ渡さなければdatabase待機を中断できないのはなぜですか。

回答:

途中のlayerがtokenを渡さなければ、その先ではcancel要求を観測することができないため


## 問2

一覧取得で`CountAsync`と`ToListAsync`の両方へ同じtokenを渡す必要があるのはなぜですか。

回答:

あるキャンセル要求が発生した際にどちらもキャンセルしたいため

## 問3

cancelされた作成処理で例外だけでなく、別scopeからdatabase件数が5件のままであることも確認するのはなぜですか。

回答:

例外が発生したという事実を確認するだけでは、DBへINSERTが保存されていないことを確認することができない。そのため別スコープで件数をセレクトし直してDBの中身まで確認している

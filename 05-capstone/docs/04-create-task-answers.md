# taskを作成して201 Createdを返す: 回答シート

教材: [04-create-task.md](04-create-task.md)

状態: 完了

## 問1

`SaveChangesAsync`の完了後に`Results.Created`を組み立てる必要があるのはなぜですか。IDと保存成否の両方に触れてください。

回答:

serviceが返す保存済みtaskのIDを、bodyとLocationの両方に使う必要があるため
また、保存失敗したのに201を返したくないため、保存完了してからレスポンスを組み立てている


## 問2

titleを`Trim`してから`IsNullOrWhiteSpace`と最大長を検証することで、保存される文字列はどのように制限されますか。

回答:

前後の空白がTrimによって除去され、IsNullOrWhiteSpaceによってnull/空文字/空白だけの文字列ではないことがvalidationされ、前段のifで100文字以下の文字列であることがチェックされる。

結果として、以下の条件を満たす文字列が保存される。

- 前後の空白がない
- null/空文字/空白ではない
- 100文字以下である


## 問3

正常系testがPOST responseだけでなく、作成後に同じIDをGETしているのは何を確認するためですか。

回答:

発行されたIDに対して、データが正常にDBに保存されていることを確認するため

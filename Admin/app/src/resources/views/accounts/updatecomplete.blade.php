<!DOCTYPE html>
<html lang="ja">
<head>
    <title>パスワード更新</title>
<body>
@if($errors->any())
    <ul>
        @foreach($errors->all() as $error)
            <li>{{$error}}</li>
        @endforeach
    </ul>
@endif
<h1>パスワード更新完了</h1>
<td>{{$name}}のパスワードを更新しました</td>
<br>
<button type="button" onclick="location.href='{{route('accountsindex')}}'"
        name="destroybutton">アカウント一覧に戻る
</button>
</body>
</html>

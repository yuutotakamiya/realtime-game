@extends('layouts.app')
@section('title','アカウント削除完了')
@section('h1','アカウント削除完了')

@section('body')
    @if($errors->any())
        <ul>
            @foreach($errors->all() as $error)
                <li>{{$error}}</li>
            @endforeach
        </ul>
    @endif
    <h1>アカウント削除完了</h1>
    <td>{{$name}}を削除完了しました</td>
    <br>
    <button type="button" onclick="location.href='{{route('accountsindex')}}'"
            name="destroybutton">アカウント一覧に戻る
    </button>
@endsection


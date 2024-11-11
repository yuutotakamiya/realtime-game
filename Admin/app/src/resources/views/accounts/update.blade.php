@extends('layouts.app')
@section('title','パスワード更新')
@section('h1','パスワード更新')

@section('body')
    @if($errors->any())
        <ul>
            @foreach($errors->all() as $error)
                <li>{{$error}}</li>
            @endforeach
        </ul>
    @endif
    <form method="post" action="{{route('accountsupdate')}}">
        @csrf
        {{$name}}<br>
        <input type="password" name="password" placeholder="パスワードを入力してください"><br>
        <input type="password" name="password_confirmation" placeholder="パスワードを再入力してください"><br>
        <button type="submit">更新</button>
        <input type="hidden" name="action" value="update">
        <input type="hidden" name="id" value={{$id}}{{$name}}>
    </form>
@endsection


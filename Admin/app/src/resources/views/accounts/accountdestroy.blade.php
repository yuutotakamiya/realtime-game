@extends('layouts.app')
@section('title','アカウント削除')
@section('h1','アカウント削除')

@section('body')
    @if($errors->any())
        <ul>
            @foreach($errors->all() as $error)
                <li>{{$error}}</li>
            @endforeach
        </ul>
    @endif
    <h1>アカウント削除</h1>
    <form method="post" action="{{route('accountsdestroy')}}">
        @csrf
        <td>{{$name}}を削除しますか？</td>
        <br>
        <button type="submit" name="name">削除</button>
        <input type="hidden" name="action" value="destroy">
        <input type="hidden" name="id" value={{$id}}>
    </form>
@endsection

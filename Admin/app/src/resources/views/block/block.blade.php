@extends('layouts.app')
@section('title','ブロック一覧')
@section('h1','ブロック一覧')
@section('body')
    @if(!empty($block))
        <table class="table table-bordered">
            <tr>
                <th>id</th>
                <th>島のID</th>
                <th>ブロックを置いたユーザーID</th>
                <th>種類</th>
                <th>Xの座標</th>
                <th>Zの座標</th>
            </tr>
            @foreach($block as $blocks)
                <tr>
                    <td>{{$blocks['id']}}</td>
                    <td>{{$blocks['land_id']}}</td>
                    <td>{{$blocks['block_user_id']}}</td>
                    <td>{{$blocks['type']}}</td>
                    <td>{{$blocks['x_Direction']}}</td>
                    <td>{{$blocks['z_Direction']}}</td>
                </tr>
            @endforeach
        </table>
    @endif
@endsection





